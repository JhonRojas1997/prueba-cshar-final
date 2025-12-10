using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Application.Services;

namespace TalentoPlus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        return services;
        return services;
    }
}
