using System.Net;
using System.Net.Http.Json;
using PasswordGenerator.Application.Contracts;

namespace PasswordGenerator.Tests;

public class GetPasswordByIdEndpointTests
{
    [Fact]
    public async Task GetApiPasswordsById_WhenPasswordExists_ReturnsOkWithPayload()
    {
        await using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var createResponse = await client.PostAsync("/api/passwords", null);
        var createdResponse = await createResponse.Content.ReadFromJsonAsync<PasswordResponse>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdResponse);

        using var readResponse = await client.GetAsync($"/api/passwords/{createdResponse!.Id}");
        var payload = await readResponse.Content.ReadFromJsonAsync<PasswordResponse>();

        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(createdResponse.Id, payload!.Id);
        Assert.Equal(createdResponse.Password, payload.Password);
    }

    [Fact]
    public async Task GetApiPasswordsById_WhenPasswordDoesNotExist_ReturnsNotFound()
    {
        await using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/passwords/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetApiPasswordsById_WhenIdIsInvalid_ReturnsBadRequest()
    {
        await using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/passwords/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
