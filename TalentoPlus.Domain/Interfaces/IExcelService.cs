using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IExcelService
{
    Task<List<Empleado>> ImportEmpleadosAsync(Stream fileStream);
}
