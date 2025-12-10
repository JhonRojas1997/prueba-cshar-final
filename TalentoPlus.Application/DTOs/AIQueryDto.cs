namespace TalentoPlus.Application.DTOs;

public class AIQueryDto
{
    public string Query { get; set; } = string.Empty;
}

public class AIResponseDto
{
    public string Response { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
}
