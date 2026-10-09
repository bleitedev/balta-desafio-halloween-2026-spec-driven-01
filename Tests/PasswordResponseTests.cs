using PasswordGenerator.Application.Contracts;

namespace PasswordGenerator.Tests;

public class PasswordResponseTests
{
    [Fact]
    public void ConstructorStoresValuesAndExposesImmutableProperties()
    {
        var id = Guid.NewGuid();
        const string password = "Strong!Password123";
        var createdAtUtc = new DateTimeOffset(2026, 10, 8, 3, 11, 54, TimeSpan.Zero);

        var response = new PasswordResponse(id, password, createdAtUtc);

        Assert.Equal(id, response.Id);
        Assert.Equal(password, response.Password);
        Assert.Equal(createdAtUtc, response.CreatedAtUtc);
    }
}
