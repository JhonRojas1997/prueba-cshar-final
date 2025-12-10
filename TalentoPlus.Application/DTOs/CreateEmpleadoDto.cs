using System;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Application.DTOs;

public class CreateEmpleadoDto
{
    [Required]
    public string Documento { get; set; } = string.Empty;
    
    [Required]
    public string Nombres { get; set; } = string.Empty;
    
    [Required]
    public string Apellidos { get; set; } = string.Empty;
    
    public DateTime FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string PerfilProfesional { get; set; } = string.Empty;
    
    // Case-insensitive string matching or Logic to find IDs will be handled in Service
    public string Cargo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string NivelEducativo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
}
