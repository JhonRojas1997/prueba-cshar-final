using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Services;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized("Credenciales inválidas.");

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateEmpleadoDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new { Message = "Registro exitoso. Se ha enviado un correo de bienvenida.", Empleado = result });
        }
        catch (System.Exception ex) // Catch simple errors like duplicate email
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
