using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmpleadoRepository
{
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<Empleado?> GetByIdAsync(int id);
    Task<Empleado?> GetByDocumentoAsync(string documento);
    Task<Empleado> AddAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(Empleado empleado);
    Task<int> GetCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetDepartmentsCountAsync();
    
    // Auxiliary methods for dropdowns/validation
    Task<IEnumerable<Departamento>> GetDepartamentosAsync();
    Task<IEnumerable<Cargo>> GetCargosAsync();
    Task<IEnumerable<NivelEducativo>> GetNivelesEducativosAsync();
}
