using Microsoft.OpenApi.Models;
using TalentoPlus.Application;
using TalentoPlus.Infrastructure;
using TalentoPlus.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalentoPlus API", Version = "v1" });
    
    // JWT Support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Clean Architecture Layers
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Ensure Database Created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try 
    {
        var context = services.GetRequiredService<TalentoPlusDbContext>();
        
        // Robust Database Connection Retry Logic
        var retries = 5;
        while (retries > 0)
        {
            try 
            {
                context.Database.EnsureCreated();
                logger.LogInformation("[DEBUG] Database ensured created.");
                break;
            }
            catch (Exception)
            {
                retries--;
                if (retries == 0) throw;
                logger.LogWarning($"[DEBUG] Waiting for Database... ({retries} retries left)");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[DEBUG] An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
    // Enable Swagger in all environments for testing purposes
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
