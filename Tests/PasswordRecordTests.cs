using PasswordGenerator.Domain.Entities;
using System.Reflection;

namespace PasswordGenerator.Tests;

public class PasswordRecordTests
{
    [Fact]
    public void ConstructorStoresValuesAndNormalizesCreationTimeToUtc()
    {
        Guid id = Guid.NewGuid();
        DateTimeOffset createdAt = new(2026, 10, 8, 12, 0, 0, TimeSpan.FromHours(-3));

        var record = new PasswordRecord(id, "Strong!Password123", createdAt);

        Assert.Equal(id, record.Id);
        Assert.Equal("Strong!Password123", record.Value);
        Assert.Equal(createdAt.ToUniversalTime(), record.CreatedAtUtc);
        Assert.Equal(TimeSpan.Zero, record.CreatedAtUtc.Offset);
    }

    [Fact]
    public void IdentityAndValuePropertiesHaveNoSetter()
    {
        PropertyInfo? idProperty = typeof(PasswordRecord).GetProperty(nameof(PasswordRecord.Id));
        PropertyInfo? valueProperty = typeof(PasswordRecord).GetProperty(nameof(PasswordRecord.Value));
        PropertyInfo? createdAtProperty = typeof(PasswordRecord).GetProperty(nameof(PasswordRecord.CreatedAtUtc));

        Assert.NotNull(idProperty);
        Assert.NotNull(valueProperty);
        Assert.NotNull(createdAtProperty);
        Assert.Null(idProperty.SetMethod);
        Assert.Null(valueProperty.SetMethod);
        Assert.Null(createdAtProperty.SetMethod);
    }
}