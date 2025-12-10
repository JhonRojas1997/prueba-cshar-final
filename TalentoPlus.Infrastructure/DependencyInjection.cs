using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Persistence;
using TalentoPlus.Infrastructure.Repositories;

using TalentoPlus.Infrastructure.Services;
using TalentoPlus.Application.Services; // For IAuthService

namespace TalentoPlus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<TalentoPlusDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity (For Web Admin AND API Employees)
        services.AddIdentity<IdentityUser, IdentityRole>(options => 
        {
            options.SignIn.RequireConfirmedAccount = false;
            // Relax password requirements to allow "Documento" as password
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 4;
        })
        .AddEntityFrameworkStores<TalentoPlusDbContext>()
        .AddDefaultTokenProviders();

        // Configure cookie authentication for web app
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true;
        });

        // Authentication (JWT for API, Cookie implicitly added by Identity)
        services.AddAuthentication(options => 
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing")))
            };
        });

        // Repositories
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

        // External Services
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IAIService, GeminiService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}
