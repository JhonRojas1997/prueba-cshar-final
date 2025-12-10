using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IEmpleadoRepository _repository;

    public DepartamentosController(IEmpleadoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deps = await _repository.GetDepartamentosAsync();
        return Ok(deps);
    }
}
