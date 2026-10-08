using System.Reflection;

namespace PasswordGenerator.Tests;

public class TestProjectConfigurationTests
{
    [Fact]
    public void ApiAssemblyIsAvailableToTests()
    {
        Assembly apiAssembly = Assembly.Load("PasswordGenerator.Api");

        Assert.Equal("PasswordGenerator.Api", apiAssembly.GetName().Name);
    }
}