using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Persistence;

public class TalentoPlusDbContext : IdentityDbContext
{
    public TalentoPlusDbContext(DbContextOptions<TalentoPlusDbContext> options) : base(options)
    {
    }

    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<NivelEducativo> NivelesEducativos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Required for Identity
        builder.Entity<Empleado>()
            .HasIndex(e => e.Documento)
            .IsUnique();
            
        builder.Entity<Empleado>()
            .HasIndex(e => e.Email)
            .IsUnique();

        // Configure decimal precision for Salary
        builder.Entity<Empleado>()
            .Property(e => e.Salario)
            .HasColumnType("decimal(18,2)");

        // Seed basic data (Reliable code-first approach)
        builder.Entity<Cargo>().HasData(
            new Cargo { Id = 1, Nombre = "Desarrollador" },
            new Cargo { Id = 2, Nombre = "Gerente" },
            new Cargo { Id = 3, Nombre = "Analista" }
        );

        builder.Entity<Departamento>().HasData(
            new Departamento { Id = 1, Nombre = "IT" },
            new Departamento { Id = 2, Nombre = "Recursos Humanos" },
            new Departamento { Id = 3, Nombre = "Contabilidad" }
        );

        builder.Entity<NivelEducativo>().HasData(
            new NivelEducativo { Id = 1, Nombre = "Profesional" },
            new NivelEducativo { Id = 2, Nombre = "Tecnólogo" },
            new NivelEducativo { Id = 3, Nombre = "Maestría" }
        );

        // Seed Admin User (Identity)
        var adminId = "8e445865-a24d-4543-a6c6-9443d048cdb9";
        var hasher = new PasswordHasher<IdentityUser>();
        
        builder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = adminId,
                UserName = "admin@talentoplus.com",
                NormalizedUserName = "ADMIN@TALENTOPLUS.COM",
                Email = "admin@talentoplus.com",
                NormalizedEmail = "ADMIN@TALENTOPLUS.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new IdentityUser("admin"), "Admin123!"),
                SecurityStamp = string.Empty
            }
        );
    }
}
