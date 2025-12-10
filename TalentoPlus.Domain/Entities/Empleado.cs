using System;

namespace TalentoPlus.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }
    
    // Identificación
    public required string Documento { get; set; }
    
    // Datos Personales
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public required string Direccion { get; set; }
    public required string Telefono { get; set; }
    public required string Email { get; set; } // Nombre en Excel: Email, Plan: Correo. Usaremos Email para coincidir con Excel.
    
    // Datos Laborales
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public required string PerfilProfesional { get; set; }
    public EstadoEmpleado Estado { get; set; }
    
    // Relaciones (Foreign Keys)
    public int CargoId { get; set; }
    public Cargo? Cargo { get; set; }
    
    public int DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }
    
    public int NivelEducativoId { get; set; }
    public NivelEducativo? NivelEducativo { get; set; }
    
    // Identidad (Login)
    public string? UsuarioId { get; set; } // Link to ASP.NET Identity User
}
