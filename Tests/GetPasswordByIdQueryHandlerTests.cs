using PasswordGenerator.Application.Contracts;
using PasswordGenerator.Application.Handlers;
using PasswordGenerator.Application.Queries;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Domain.Interfaces;

namespace PasswordGenerator.Tests;

public class GetPasswordByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenPasswordExists_ReturnsPasswordResponse()
    {
        var id = Guid.NewGuid();
        var createdAtUtc = new DateTimeOffset(2026, 10, 8, 3, 11, 54, TimeSpan.Zero);
        var passwordRecord = new PasswordRecord(id, "Strong!Password123", createdAtUtc);
        var repository = new FakePasswordRepository(passwordRecord);
        var handler = new GetPasswordByIdQueryHandler(repository);

        PasswordResponse? response = await handler.HandleAsync(new GetPasswordByIdQuery(id));

        Assert.NotNull(response);
        Assert.Equal(id, response!.Id);
        Assert.Equal(passwordRecord.Value, response.Password);
        Assert.Equal(createdAtUtc, response.CreatedAtUtc);
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordDoesNotExist_ReturnsNull()
    {
        var repository = new FakePasswordRepository();
        var handler = new GetPasswordByIdQueryHandler(repository);

        PasswordResponse? response = await handler.HandleAsync(new GetPasswordByIdQuery(Guid.NewGuid()));

        Assert.Null(response);
    }

    private sealed class FakePasswordRepository : IPasswordRepository
    {
        private readonly PasswordRecord? _passwordRecord;

        public FakePasswordRepository(PasswordRecord? passwordRecord = null)
        {
            _passwordRecord = passwordRecord;
        }

        public void Add(PasswordRecord passwordRecord)
        {
            throw new NotSupportedException();
        }

        public Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_passwordRecord is null)
            {
                return Task.FromResult<PasswordRecord?>(null);
            }

            return _passwordRecord.Id == id
                ? Task.FromResult<PasswordRecord?>(_passwordRecord)
                : Task.FromResult<PasswordRecord?>(null);
        }
    }
}
