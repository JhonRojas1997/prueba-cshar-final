using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Web.Models;

public class EmpleadoFormViewModel
{
    public Empleado Empleado { get; set; } = new Empleado 
    { 
        Documento = "", Nombres = "", Apellidos = "", Direccion = "", Telefono = "", Email = "", PerfilProfesional = "" 
    };

    // Dropdowns
    public IEnumerable<SelectListItem> Cargos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Departamentos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> NivelesEducativos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Estados { get; set; } = new List<SelectListItem>();
}
