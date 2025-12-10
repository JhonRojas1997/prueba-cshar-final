using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Services;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Controllers;

[Authorize]
public class EmpleadosController : Controller
{
    private readonly IExcelService _excelService;
    private readonly IEmpleadoRepository _repository;
    private readonly IPdfService _pdfService;
    private readonly IAuthService _authService;

    public EmpleadosController(IExcelService excelService, IEmpleadoRepository repository, IPdfService pdfService, IAuthService authService)
    {
        _excelService = excelService;
        _repository = repository;
        _pdfService = pdfService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searchString, int page = 1)
    {
        var empleados = await _repository.GetAllAsync();
        
        if (!string.IsNullOrEmpty(searchString))
        {
            empleados = empleados.Where(e => 
                e.Nombres.Contains(searchString, StringComparison.OrdinalIgnoreCase) || 
                e.Apellidos.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                e.Documento.Contains(searchString));
        }

        // Simple client-side pagination for now (or implement server-side later)
        int pageSize = 10;
        var pagedData = empleados.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(empleados.Count() / (double)pageSize);
        ViewBag.SearchString = searchString;

        return View(pagedData);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await LoadFormViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmpleadoFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Manual validation bypass for simplicity if model binding is tricky with Entity
            // Re-bind complex properties if needed, or better: use InputModel
            
            // Set UTC dates
            model.Empleado.FechaIngreso = DateTime.SpecifyKind(model.Empleado.FechaIngreso, DateTimeKind.Utc);
            model.Empleado.FechaNacimiento = DateTime.SpecifyKind(model.Empleado.FechaNacimiento, DateTimeKind.Utc);

            // Fetch Names for DTO (since Service expects strings to lookup)
            var cargos = await _repository.GetCargosAsync();
            var deptos = await _repository.GetDepartamentosAsync();
            var niveles = await _repository.GetNivelesEducativosAsync();

            var cargoName = cargos.FirstOrDefault(c => c.Id == model.Empleado.CargoId)?.Nombre ?? "";
            var deptoName = deptos.FirstOrDefault(d => d.Id == model.Empleado.DepartamentoId)?.Nombre ?? "";
            var nivelName = niveles.FirstOrDefault(n => n.Id == model.Empleado.NivelEducativoId)?.Nombre ?? "";

            var dto = new CreateEmpleadoDto
            {
                Documento = model.Empleado.Documento,
                Nombres = model.Empleado.Nombres,
                Apellidos = model.Empleado.Apellidos,
                Email = model.Empleado.Email,
                Salario = model.Empleado.Salario,
                FechaNacimiento = model.Empleado.FechaNacimiento,
                FechaIngreso = model.Empleado.FechaIngreso,
                Direccion = model.Empleado.Direccion,
                Telefono = model.Empleado.Telefono,
                PerfilProfesional = model.Empleado.PerfilProfesional ?? "",
                Estado = model.Empleado.Estado.ToString(),
                Cargo = cargoName,
                Departamento = deptoName,
                NivelEducativo = nivelName
            };

            await _authService.RegisterAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        
        // Reload dropdowns if failed
        var freshModel = await LoadFormViewModel();
        freshModel.Empleado = model.Empleado;
        return View(freshModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado == null) return NotFound();

        var model = await LoadFormViewModel();
        model.Empleado = empleado;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EmpleadoFormViewModel model)
    {
        if (id != model.Empleado.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                 // Ensure UTC
                model.Empleado.FechaIngreso = DateTime.SpecifyKind(model.Empleado.FechaIngreso, DateTimeKind.Utc);
                model.Empleado.FechaNacimiento = DateTime.SpecifyKind(model.Empleado.FechaNacimiento, DateTimeKind.Utc);
                
                await _repository.UpdateAsync(model.Empleado);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Log
                ModelState.AddModelError("", "Error al actualizar.");
            }
        }

        var freshModel = await LoadFormViewModel();
        freshModel.Empleado = model.Empleado;
        return View(freshModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado != null)
        {
            // Soft Delete: Change status to Inactivo
            empleado.Estado = EstadoEmpleado.Inactivo;
            await _repository.UpdateAsync(empleado);
        }
        return RedirectToAction(nameof(Index));
    }

    // Helpers
    private async Task<EmpleadoFormViewModel> LoadFormViewModel()
    {
        var viewModel = new EmpleadoFormViewModel();
        
        var cargos = await _repository.GetCargosAsync();
        viewModel.Cargos = cargos.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre });

        var deptos = await _repository.GetDepartamentosAsync();
        viewModel.Departamentos = deptos.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre });

        var niveles = await _repository.GetNivelesEducativosAsync();
        viewModel.NivelesEducativos = niveles.Select(n => new SelectListItem { Value = n.Id.ToString(), Text = n.Nombre });

        viewModel.Estados = Enum.GetValues(typeof(TalentoPlus.Domain.Entities.EstadoEmpleado))
            .Cast<TalentoPlus.Domain.Entities.EstadoEmpleado>()
            .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

        return viewModel;
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            ModelState.AddModelError("", "Por favor seleccione un archivo válido.");
            return View();
        }

        try
        {
            using var stream = archivo.OpenReadStream();
            var results = await _excelService.ImportEmpleadosAsync(stream);
            TempData["SuccessMessage"] = $"Se importaron {results.Count} empleados exitosamente.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al importar: {ex.Message}");
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadCv(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado == null) return NotFound();

        var pdfBytes = _pdfService.GenerateHojaDeVida(empleado);
        return File(pdfBytes, "application/pdf", $"CV_{empleado.Nombres}_{empleado.Apellidos}.pdf");
    }
}
