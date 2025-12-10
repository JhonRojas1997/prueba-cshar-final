using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using System.Text;
using System.Globalization;
using TalentoPlus.Infrastructure.Persistence;

namespace TalentoPlus.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly TalentoPlusDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ExcelService(TalentoPlusDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<Empleado>> ImportEmpleadosAsync(Stream fileStream)
    {
        var empleados = new List<Empleado>();

        // Ensure stream is at beginning
        if (fileStream.CanSeek) fileStream.Position = 0;

        using (var workbook = new XLWorkbook(fileStream))
        {
            var worksheet = workbook.Worksheet(1);
            var range = worksheet.RangeUsed();
            if (range != null)
            {
                // Dynamic Header Mapping
                var headerRow = range.Row(1);
                var headers = new Dictionary<string, int>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    headers[cell.GetValue<string>().Trim().ToLower()] = cell.Address.ColumnNumber;
                }

                // Helper to get value by header name
                Func<IXLRangeRow, string, string> getStr = (r, name) => 
                {
                    foreach(var key in headers.Keys) 
                        if(key.Contains(name)) 
                            return r.Cell(headers[key]).GetValue<string>();
                    return "";
                };

                Func<IXLRangeRow, string, decimal> getDec = (r, name) => 
                {
                     foreach(var key in headers.Keys) 
                        if(key.Contains(name)) 
                            return r.Cell(headers[key]).GetValue<decimal>();
                    return 0;
                };

                 var rows = range.RowsUsed().Skip(1); // Skip header

                foreach (var row in rows)
                {
                    try 
                    {
                        var documento = getStr(row, "documento");
                        var nombres = getStr(row, "nombre");
                        var apellidos = getStr(row, "apellido");
                        var cargoNombre = getStr(row, "cargo");
                        var deptoNombre = getStr(row, "departamento");
                        var salario = getDec(row, "salario");
                        
                        // Robust Date Parsing for Fecha Ingreso
                        DateTime fechaIngreso = DateTime.UtcNow;
                        foreach(var key in headers.Keys) 
                        {
                            if(key.Contains("ingreso") || key.Contains("fecha"))
                            {
                                var cell = row.Cell(headers[key]);
                                if (!cell.TryGetValue(out fechaIngreso))
                                {
                                     var fStr = cell.GetValue<string>();
                                     DateTime.TryParse(fStr, out fechaIngreso);
                                }
                                break;
                            }
                        }

                        var nivelNombre = getStr(row, "nivel") ?? getStr(row, "educativo");
                        var email = getStr(row, "email") ?? getStr(row, "correo");
                        var direccion = getStr(row, "direccion");
                        var telefono = getStr(row, "telefono") ?? getStr(row, "celular");

                        // Validate minimal data
                        if (string.IsNullOrEmpty(documento) || string.IsNullOrEmpty(nombres)) continue;

                        // Find or Create Related Entities
                        var cargo = await GetOrCreateCargoAsync(string.IsNullOrEmpty(cargoNombre) ? "Sin Cargo" : cargoNombre);
                        var depto = await GetOrCreateDepartamentoAsync(string.IsNullOrEmpty(deptoNombre) ? "Sin Departamento" : deptoNombre);
                        var nivel = await GetOrCreateNivelAsync(string.IsNullOrEmpty(nivelNombre) ? "Bacillerato" : nivelNombre);



                        // Normalize Email: Remove accents, lower case, trim
                        var cleanEmail = RemoveDiacritics(string.IsNullOrEmpty(email) ? $"{documento}@empresa.com" : email).ToLower().Trim();

                        var empleado = new Empleado
                        {
                            Documento = documento.Trim(),
                            Nombres = nombres.Trim(),
                            Apellidos = apellidos.Trim(),
                            Email = cleanEmail,
                            Salario = salario,
                            FechaIngreso = DateTime.SpecifyKind(fechaIngreso, DateTimeKind.Utc),
                            CargoId = cargo.Id,
                            DepartamentoId = depto.Id,
                            NivelEducativoId = nivel.Id,
                            Direccion = string.IsNullOrEmpty(direccion) ? "No registrada" : direccion,
                            Telefono = string.IsNullOrEmpty(telefono) ? "0000000" : telefono,
                            FechaNacimiento = DateTime.UtcNow.AddYears(-20), 
                            PerfilProfesional = "Importado",
                            Estado = EstadoEmpleado.Activo
                        };

                        _context.Empleados.Add(empleado);
                        empleados.Add(empleado);

                        // CREATE IDENTITY USER (Sync for Login)
                        // Note: Using document as temporary password as per requirement logic
                        var user = new IdentityUser { UserName = cleanEmail, Email = cleanEmail };
                        // We use .Result or Wait() here cautiously because ExcelService is async but loop logic inside might be tricky if mixed.
                        // Actually, ImportEmpleadosAsync is Task, we can use await.
                        
                        var existingUser = await _userManager.FindByEmailAsync(cleanEmail);
                        if (existingUser == null)
                        {
                            var result = await _userManager.CreateAsync(user, empleado.Documento);
                            if (!result.Succeeded)
                            {
                                Console.WriteLine($"Identity Create Failed for {cleanEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing row {row.RowNumber()}: {ex.Message}");
                    }
                }
                await _context.SaveChangesAsync();
            }
        }

        return empleados;
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private async Task<Cargo> GetOrCreateCargoAsync(string nombre)
    {
        var entity = await _context.Cargos.FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (entity == null)
        {
            entity = new Cargo { Nombre = nombre };
            _context.Cargos.Add(entity);
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    private async Task<Departamento> GetOrCreateDepartamentoAsync(string nombre)
    {
        var entity = await _context.Departamentos.FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (entity == null)
        {
            entity = new Departamento { Nombre = nombre };
            _context.Departamentos.Add(entity);
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    private async Task<NivelEducativo> GetOrCreateNivelAsync(string nombre)
    {
        var entity = await _context.NivelesEducativos.FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (entity == null)
        {
            entity = new NivelEducativo { Nombre = nombre };
            _context.NivelesEducativos.Add(entity);
            await _context.SaveChangesAsync();
        }
        return entity;
    }
}
