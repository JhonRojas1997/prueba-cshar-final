using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Services;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllAsync();
    Task<EmpleadoDto?> GetByIdAsync(int id);
    Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto);
    Task UpdateAsync(int id, CreateEmpleadoDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> GenerateCvPdfAsync(int id);
}
