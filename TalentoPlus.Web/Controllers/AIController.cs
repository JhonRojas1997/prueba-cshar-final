using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize]
public class AIController : Controller
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Query([FromBody] AIQueryDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new AIResponseDto
            {
                Response = "La consulta no puede estar vacía.",
                Query = request.Query
            });
        }

        var response = await _aiService.ProcessNaturalLanguageQueryAsync(request.Query);

        return Ok(new AIResponseDto
        {
            Response = response,
            Query = request.Query
        });
    }
}
