using TalentoPlus.Application;
using TalentoPlus.Infrastructure;
using TalentoPlus.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
        var config = services.GetRequiredService<IConfiguration>();
        var conn = config.GetConnectionString("DefaultConnection");
        
        logger.LogInformation($"[DEBUG] Connection String Found: {!string.IsNullOrEmpty(conn)}");
        if (string.IsNullOrEmpty(conn)) logger.LogError("[DEBUG] Connection String is NULL or EMPTY!");

        // context.Database.Migrate(); // Use this if migrations enabled
        context.Database.EnsureCreated(); // Use this for quick prototype
        logger.LogInformation("[DEBUG] Database ensured created.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[DEBUG] An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
