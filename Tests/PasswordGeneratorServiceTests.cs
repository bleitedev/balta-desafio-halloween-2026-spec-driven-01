using PasswordGenerator.Domain.Services;

namespace PasswordGenerator.Tests;

public class PasswordGeneratorServiceTests
{
    [Fact]
    public void GeneratePasswordReturnsStrongPasswordWithoutWhitespace()
    {
        var service = new PasswordGeneratorService();

        string password = service.GeneratePassword();

        Assert.True(password.Length >= 16);
        Assert.Contains(password, char.IsUpper);
        Assert.Contains(password, char.IsLower);
        Assert.Contains(password, char.IsDigit);
        Assert.Contains(password, character => !char.IsLetterOrDigit(character));
        Assert.DoesNotContain(password, char.IsWhiteSpace);
    }
}
