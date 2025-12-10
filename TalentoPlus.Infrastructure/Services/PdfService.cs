using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        // License configuration (Community is free for individuals and small companies < 1M revenue)
        // Adjust based on real usage scenario, assuming community/educational for this test.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateHojaDeVida(Empleado empleado)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(0);
                
                page.Header()
                    .Height(100)
                    .Background(Colors.Blue.Darken2)
                    .Padding(20)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text($"{empleado.Nombres} {empleado.Apellidos}")
                                .FontSize(26).SemiBold().FontColor(Colors.White);
                            column.Item().Text(empleado.Cargo?.Nombre ?? "Cargo no especificado")
                                .FontSize(16).FontColor(Colors.Grey.Lighten2);
                        });
                    });

                page.Content()
                    .Row(row =>
                    {
                        // Left Sidebar (Personal Info)
                        row.ConstantItem(250).Background(Colors.Grey.Lighten4).Padding(20).Column(column =>
                        {
                            column.Item().Text("Datos Personales").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                            column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
                            
                            column.Item().Text("Documento ID:");
                            column.Item().Text(empleado.Documento).FontSize(10);
                            column.Item().PaddingBottom(10);

                            column.Item().Text("Fecha Nacimiento:");
                            column.Item().Text($"{empleado.FechaNacimiento:dd/MM/yyyy}").FontSize(10);
                            column.Item().PaddingBottom(20);

                            column.Item().Text("Contacto").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                            column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Blue.Darken2);

                            column.Item().Text("Email:");
                            column.Item().Text(empleado.Email).FontSize(10);
                            column.Item().PaddingBottom(10);

                            column.Item().Text("Teléfono:");
                            column.Item().Text(empleado.Telefono).FontSize(10);
                            column.Item().PaddingBottom(10);
                            
                            column.Item().Text("Dirección:");
                            column.Item().Text(empleado.Direccion).FontSize(10);
                        });

                        // Main Content
                        row.RelativeItem().Padding(20).Column(column =>
                        {
                            // Perfil
                            column.Item().Text("Perfil Profesional").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken3);
                            column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                            column.Item().Text(empleado.PerfilProfesional ?? "Sin perfil registrado.")
                                .FontSize(11).Justify();
                            column.Item().PaddingBottom(20);

                            // Información Laboral
                            column.Item().Text("Información Corporativa").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken3);
                            column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                            
                            column.Item().Row(r => 
                            { 
                                r.RelativeItem().Text("Departamento:").Bold(); 
                                r.RelativeItem().Text(empleado.Departamento?.Nombre ?? "N/A"); 
                            });
                            column.Item().Row(r => 
                            { 
                                r.RelativeItem().Text("Fecha Ingreso:").Bold(); 
                                r.RelativeItem().Text($"{empleado.FechaIngreso:dd/MM/yyyy}"); 
                            });
                             column.Item().Row(r => 
                            { 
                                r.RelativeItem().Text("Salario Actual:").Bold(); 
                                r.RelativeItem().Text($"{empleado.Salario:C}"); 
                            });
                            column.Item().Row(r => 
                            { 
                                r.RelativeItem().Text("Nivel Educativo:").Bold(); 
                                r.RelativeItem().Text(empleado.NivelEducativo?.Nombre ?? "N/A"); 
                            });
                            column.Item().PaddingBottom(20);

                            column.Item().Background(Colors.Grey.Lighten5).Padding(10).Column(c =>
                            {
                                c.Item().Text("Estado del Empleado").FontSize(12).SemiBold();
                                c.Item().Text(empleado.Estado.ToString()).FontSize(14).FontColor(Colors.Green.Darken2);
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generado automáticamente por TalentoPlus - ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf();
    }
}
