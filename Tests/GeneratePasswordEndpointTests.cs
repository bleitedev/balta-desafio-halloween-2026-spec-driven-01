using System.Net;
using System.Net.Http.Json;
using PasswordGenerator.Application.Contracts;

namespace PasswordGenerator.Tests;

public class GeneratePasswordEndpointTests
{
    [Fact]
    public async Task PostApiPasswords_ReturnsCreatedWithPasswordResponse()
    {
        await using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync("/api/passwords", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("/api/passwords/", response.Headers.Location!.OriginalString.Substring(0, "/api/passwords/".Length));

        var payload = await response.Content.ReadFromJsonAsync<PasswordResponse>();

        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload!.Id);
        Assert.True(payload.Password.Length >= 16);
        Assert.DoesNotContain(payload.Password, char.IsWhiteSpace);
        Assert.Contains(payload.Password, c => !char.IsLetterOrDigit(c));
    }
}
