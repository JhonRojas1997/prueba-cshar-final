using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TalentoPlus.Api;

namespace TalentoPlus.Tests.IntegrationTests;

public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RootEndpoint_ReturnsResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert - Should return either OK or NotFound, but not server error
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected OK or NotFound but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task SwaggerEndpoint_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/index.html");

        // Assert
        Assert.True(
            response.IsSuccessStatusCode,
            "Swagger UI should be accessible"
        );
    }

    [Fact]
    public async Task ApiEmpleados_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/empleados");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ApiAuthLogin_AcceptsPostRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginRequest = new
        {
            documento = "testuser",
            password = "testpassword"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync("/api/auth/login", content);

        // Assert - Should return either OK or Unauthorized, not server error
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Login endpoint should handle requests, got {response.StatusCode}"
        );
    }
}

