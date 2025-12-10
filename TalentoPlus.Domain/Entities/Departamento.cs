using System.Collections.Generic;

namespace TalentoPlus.Domain.Entities;

public class Departamento
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
