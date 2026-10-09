using PasswordGenerator.Application.Commands;
using PasswordGenerator.Application.Contracts;
using PasswordGenerator.Application.Handlers;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Domain.Interfaces;
using PasswordGenerator.Domain.Services;

namespace PasswordGenerator.Tests;

public class GeneratePasswordCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_GeneratesPassword_StoresRecord_AndCommitsChanges()
    {
        var passwordGeneratorService = new PasswordGeneratorService();
        var repository = new FakePasswordRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new GeneratePasswordCommandHandler(passwordGeneratorService, repository, unitOfWork);

        PasswordResponse response = await handler.HandleAsync(new GeneratePasswordCommand());

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.NotNull(response.Password);
        Assert.True(response.Password.Length >= 16);
        Assert.DoesNotContain(response.Password, char.IsWhiteSpace);
        Assert.Contains(response.Password, c => !char.IsLetterOrDigit(c));
        Assert.True(repository.AddedRecords.Count == 1);
        Assert.Equal(repository.AddedRecords[0].Id, response.Id);
        Assert.Equal(repository.AddedRecords[0].Value, response.Password);
        Assert.Equal(1, unitOfWork.CommitCount);
    }

    private sealed class FakePasswordRepository : IPasswordRepository
    {
        public List<PasswordRecord> AddedRecords { get; } = new();

        public void Add(PasswordRecord passwordRecord)
        {
            AddedRecords.Add(passwordRecord);
        }

        public Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PasswordRecord?>(AddedRecords.FirstOrDefault(record => record.Id == id));
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int CommitCount { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            CommitCount++;
            return Task.CompletedTask;
        }
    }
}
