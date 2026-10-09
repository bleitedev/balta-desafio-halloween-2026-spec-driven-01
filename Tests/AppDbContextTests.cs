using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Infrastructure.Data;

namespace PasswordGenerator.Tests;

public class AppDbContextTests
{
    [Fact]
    public void ModelMapsPasswordRecordToPasswordsTableWithExpectedColumns()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new AppDbContext(options);

        var entity = context.Model.FindEntityType(typeof(PasswordRecord));

        Assert.NotNull(entity);
        Assert.Equal("Passwords", entity.GetTableName());

        var key = entity.FindPrimaryKey();
        Assert.NotNull(key);
        Assert.Equal(nameof(PasswordRecord.Id), key.Properties.Single().Name);

        var valueProperty = entity.FindProperty(nameof(PasswordRecord.Value));
        Assert.NotNull(valueProperty);
        Assert.False(valueProperty.IsNullable);
        Assert.Equal(128, valueProperty.GetMaxLength());

        var createdAtProperty = entity.FindProperty(nameof(PasswordRecord.CreatedAtUtc));
        Assert.NotNull(createdAtProperty);
        Assert.False(createdAtProperty.IsNullable);
    }
}
