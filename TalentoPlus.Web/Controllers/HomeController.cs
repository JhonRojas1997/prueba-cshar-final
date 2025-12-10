using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TalentoPlus.Domain.Interfaces.IEmpleadoRepository _repository;
    private readonly TalentoPlus.Domain.Interfaces.IAIService _aiService;

    public HomeController(ILogger<HomeController> logger, 
                          TalentoPlus.Domain.Interfaces.IEmpleadoRepository repository,
                          TalentoPlus.Domain.Interfaces.IAIService aiService)
    {
        _logger = logger;
        _repository = repository;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var count = await _repository.GetCountAsync();
        var activeCount = await _repository.GetActiveCountAsync();
        var deptCount = await _repository.GetDepartmentsCountAsync();

        var model = new DashboardViewModel
        {
            TotalEmpleados = count,
            EmpleadosActivos = activeCount, 
            DepartamentosCount = deptCount
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AskAI([FromBody] AskAiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query)) return BadRequest("Query required");
        
        var response = await _aiService.ProcessNaturalLanguageQueryAsync(request.Query);
        return Ok(new { Answer = response });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
