using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Infrastructure.Data;

namespace PasswordGenerator.Tests;

public class PasswordRepositoryTests
{
    [Fact]
    public async Task AddAndFindByIdAsync_PersistsAndRetrievesRecord()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new PasswordRepository(context);
            var passwordRecord = new PasswordRecord(Guid.NewGuid(), "Str0ng!Password123", DateTimeOffset.UtcNow);

            repository.Add(passwordRecord);
            await context.SaveChangesAsync();

            var found = await repository.FindByIdAsync(passwordRecord.Id);

            Assert.NotNull(found);
            Assert.Equal(passwordRecord.Id, found!.Id);
            Assert.Equal(passwordRecord.Value, found.Value);
        }
    }

    [Fact]
    public async Task FindByIdAsync_WhenRecordDoesNotExist_ReturnsNull()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PasswordRepository(context);

        var found = await repository.FindByIdAsync(Guid.NewGuid());

        Assert.Null(found);
    }
}

public class UnitOfWorkTests
{
    [Fact]
    public async Task CommitAsync_SavesPendingChanges()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PasswordRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var passwordRecord = new PasswordRecord(Guid.NewGuid(), "Another!Strong123", DateTimeOffset.UtcNow);

        repository.Add(passwordRecord);
        await unitOfWork.CommitAsync();

        var saved = await context.Passwords.SingleAsync(record => record.Id == passwordRecord.Id);

        Assert.Equal(passwordRecord.Id, saved.Id);
        Assert.Equal(passwordRecord.Value, saved.Value);
    }
}
