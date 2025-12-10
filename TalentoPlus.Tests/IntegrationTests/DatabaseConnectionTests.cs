using Xunit;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Persistence;
using TalentoPlus.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace TalentoPlus.Tests.IntegrationTests;

public class DatabaseConnectionTests
{
    [Fact]
    public async Task CanConnectToDatabase_AndCreateTables()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TalentoPlusDbContext>()
            .UseInMemoryDatabase(databaseName: "TalentoPlusTestDb_Connection")
            .Options;

        // Act &Assert
        using (var context = new TalentoPlusDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();

            // Add master data
            var departamento = new Departamento { Nombre = "IT" };
            var cargo = new Cargo { Nombre = "Desarrollador" };
            var nivelEducativo = new NivelEducativo { Nombre = "Universitario" };
            
            context.Departamentos.Add(departamento);
            context.Cargos.Add(cargo);
            context.NivelesEducativos.Add(nivelEducativo);
            await context.SaveChangesAsync();

            var empleado = new Empleado
            {
                Nombres = "Test",
                Apellidos = "Integration",
                Documento = "999999",
                Email = "test@integration.com",
                Telefono = "1234567890",
                Direccion = "Test Address",
                PerfilProfesional = "Tester",
                Salario = 3000,
                FechaIngreso = DateTime.Now,
                FechaNacimiento = new DateTime(1990, 1, 1),
                Estado = EstadoEmpleado.Activo,
                DepartamentoId = departamento.Id,
                CargoId = cargo.Id,
                NivelEducativoId = nivelEducativo.Id
            };

            context.Empleados.Add(empleado);
            await context.SaveChangesAsync();

            // Query
            var empleados = await context.Empleados.ToListAsync();

            // Assert
            Assert.NotNull(empleados);
            Assert.Single(empleados);
            Assert.Equal("Test", empleados[0].Nombres);
        }
    }

    [Fact]
    public async Task CanPerformCrudOperations_OnDepartamentos()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TalentoPlusDbContext>()
            .UseInMemoryDatabase(databaseName: "TalentoPlusTestDb_CRUD")
            .Options;

        using (var context = new TalentoPlusDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();

            // CREATE
            var departamento = new Departamento { Nombre = "Recursos Humanos" };
            context.Departamentos.Add(departamento);
            await context.SaveChangesAsync();

            // READ
            var found = await context.Departamentos.FindAsync(departamento.Id);
            Assert.NotNull(found);
            Assert.Equal("Recursos Humanos", found.Nombre);

            // UPDATE
            found.Nombre = "RRHH";
            await context.SaveChangesAsync();

            var updated = await context.Departamentos.FindAsync(departamento.Id);
            Assert.Equal("RRHH", updated?.Nombre);

            // DELETE
            context.Departamentos.Remove(found);
            await context.SaveChangesAsync();

            var deleted = await context.Departamentos.FindAsync(departamento.Id);
            Assert.Null(deleted);
        }
    }
}
