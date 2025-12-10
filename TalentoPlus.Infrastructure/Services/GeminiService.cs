using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GenerativeAI;
using GenerativeAI.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Persistence;

namespace TalentoPlus.Infrastructure.Services;

public class GeminiService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly TalentoPlusDbContext _context;
    private readonly string _apiKey;

    public GeminiService(IConfiguration configuration, TalentoPlusDbContext context)
    {
        _configuration = configuration;
        _context = context;
        
        _apiKey = _configuration["Gemini:ApiKey"] ?? "";
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("Gemini API Key no configurada.");
    }

    public async Task<string> ProcessNaturalLanguageQueryAsync(string query)
    {
        try
        {
            var googleAI = new GoogleAi(_apiKey);
            var model = googleAI.CreateGenerativeModel(GoogleAIModels.Gemini2Flash);
            
            // Configure system instruction
            model.SystemInstruction = "Eres un asistente de RRHH que ayuda a consultar información sobre empleados. Responde en español de forma concisa y profesional.";
            
            // Enable function calling with auto-execution
            model.FunctionCallingBehaviour.AutoCallFunction = true;
            model.FunctionCallingBehaviour.AutoReplyFunction = true;
            
            // Add function tools with explicit Func types
            Func<string, Task<object>> countByCargoFunc = async (cargo) =>
            {
                var count = await _context.Empleados
                    .Include(e => e.Cargo)
                    .CountAsync(e => e.Cargo!.Nombre.ToLower().Contains(cargo.ToLower()));
                return new { count, cargo };
            };
            
            var countByCargoTool = new QuickTool(
                countByCargoFunc,
                "count_employees_by_cargo",
                "Cuenta el número de empleados que tienen un cargo específico"
            );

            Func<string, Task<object>> countByDepartmentFunc = async (departamento) =>
            {
                var count = await _context.Empleados
                    .Include(e => e.Departamento)
                    .CountAsync(e => e.Departamento!.Nombre.ToLower().Contains(departamento.ToLower()));
                return new { count, departamento };
            };
            
            var countByDepartmentTool = new QuickTool(
                countByDepartmentFunc,
                "count_employees_by_department",
                "Cuenta el número de empleados en un departamento específico"
            );

            Func<string, Task<object>> countByStateFunc = async (estado) =>
            {
                if (!Enum.TryParse<Domain.Entities.EstadoEmpleado>(estado, true, out var estadoEnum))
                {
                    return new { count = 0, estado, error = "Estado no válido" };
                }
                var count = await _context.Empleados.CountAsync(e => e.Estado == estadoEnum);
                return new { count, estado };
            };
            
            var countByStateTool = new QuickTool(
                countByStateFunc,
                "count_employees_by_state",
                "Cuenta el número de empleados en un estado específico"
            );

            Func<Task<object>> getAllStatsFunc = async () =>
            {
                var total = await _context.Empleados.CountAsync();
                var activos = await _context.Empleados.CountAsync(e => e.Estado == Domain.Entities.EstadoEmpleado.Activo);
                var departamentos = await _context.Departamentos.CountAsync();
                var cargos = await _context.Cargos.CountAsync();
                return new { total, activos, departamentos, cargos };
            };
            
            var getAllStatisticsTool = new QuickTool(
                getAllStatsFunc,
                "get_all_statistics",
                "Obtiene estadísticas generales de todos los empleados"
            );

            model.AddFunctionTool(countByCargoTool);
            model.AddFunctionTool(countByDepartmentTool);
            model.AddFunctionTool(countByStateTool);
            model.AddFunctionTool(getAllStatisticsTool);

            var result = await model.GenerateContentAsync(query);
            return result?.Text ?? "No se pudo generar una respuesta.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
