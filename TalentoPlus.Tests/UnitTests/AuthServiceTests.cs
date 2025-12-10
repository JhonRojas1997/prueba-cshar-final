using Xunit;

namespace TalentoPlus.Tests.UnitTests;

public class BasicEntityTests
{
    [Fact]
    public void String_Concatenation_WorksCorrectly()
    {
        // Arrange
        var firstName = "Juan";
        var lastName = "Pérez";

        // Act
        var fullName = $"{firstName} {lastName}";

        // Assert
        Assert.Equal("Juan Pérez", fullName);
    }

    [Fact]
    public void Number_Addition_WorksCorrectly()
    {
        // Arrange
        var a = 5;
        var b = 3;

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(8, result);
    }
}
