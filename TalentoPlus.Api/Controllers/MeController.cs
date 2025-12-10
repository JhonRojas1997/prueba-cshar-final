using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;

namespace TalentoPlus.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class MeController : ControllerBase
{
    private readonly IEmpleadoService _service;

    public MeController(IEmpleadoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out var id))
            return Unauthorized();

        var empleado = await _service.GetByIdAsync(id);
        if (empleado == null) return NotFound();

        return Ok(empleado);
    }

    [HttpGet("cv")]
    public async Task<IActionResult> DownloadCv()
    {
        var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out var id))
            return Unauthorized();

        try 
        {
            var pdfBytes = await _service.GenerateCvPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"HojaDeVida_{id}.pdf");
        }
        catch (System.Exception)
        {
             return NotFound("No se pudo generar la Hoja de Vida.");
        }
    }
}
