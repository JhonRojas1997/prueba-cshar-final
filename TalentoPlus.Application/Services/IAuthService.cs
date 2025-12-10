using System.Threading.Tasks;
using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(AuthRequest request);
    Task<EmpleadoDto> RegisterAsync(CreateEmpleadoDto request);
}
