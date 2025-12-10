using Xunit;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Tests.UnitTests;

public class EmpleadoEntityTests
{
    [Fact]
    public void Empleado_ShouldInitializeWithCorrectValues()
    {
        // Arrange & Act
        var empleado = new Empleado
        {
            Id = 1,
            Nombres = "Juan Carlos",
            Apellidos = "Pérez García",
            Documento = "123456789",
            Email = "juan@example.com",
            Telefono = "1234567890",
            Direccion = "Calle 123",
            PerfilProfesional = "Ingeniero de Software",
            Estado = EstadoEmpleado.Activo,
            Salario = 5000.00m,
            FechaIngreso = DateTime.Now,
            FechaNacimiento = new DateTime(1990, 1, 1),
            CargoId = 1,
            DepartamentoId = 1,
            NivelEducativoId = 1
        };

        // Assert
        Assert.Equal(1, empleado.Id);
        Assert.Equal("Juan Carlos", empleado.Nombres);
        Assert.Equal("Pérez García", empleado.Apellidos);
        Assert.Equal("123456789", empleado.Documento);
        Assert.Equal(EstadoEmpleado.Activo, empleado.Estado);
    }

    [Fact]
    public void EstadoEmpleado_ShouldHaveCorrectEnumValues()
    {
        // Assert
        Assert.Equal(0, (int)EstadoEmpleado.Activo);
        Assert.Equal(1, (int)EstadoEmpleado.Inactivo);
        Assert.Equal(2, (int)EstadoEmpleado.Vacaciones);
    }
}
