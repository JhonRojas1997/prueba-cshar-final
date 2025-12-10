namespace TalentoPlus.Domain.Entities;

public class NivelEducativo
{
    public int Id { get; set; }
    public required string Nombre { get; set; } // e.g. "Bachiller", "Profesional"
    
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
