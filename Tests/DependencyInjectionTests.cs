using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PasswordGenerator.Domain.Interfaces;
using PasswordGenerator.Domain.Services;
using PasswordGenerator.Infrastructure.Data;

namespace PasswordGenerator.Tests;

public class DependencyInjectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DependencyInjectionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Services_ResolveExpectedDependencies()
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        Assert.NotNull(services.GetRequiredService<AppDbContext>());
        Assert.NotNull(services.GetRequiredService<PasswordGeneratorService>());
        Assert.NotNull(services.GetRequiredService<IPasswordRepository>());
        Assert.NotNull(services.GetRequiredService<IUnitOfWork>());
    }
}
