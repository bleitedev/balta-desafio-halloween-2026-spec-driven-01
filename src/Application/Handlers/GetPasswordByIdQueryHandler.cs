using PasswordGenerator.Application.Contracts;
using PasswordGenerator.Application.Queries;
using PasswordGenerator.Domain.Interfaces;

namespace PasswordGenerator.Application.Handlers;

public sealed class GetPasswordByIdQueryHandler
{
    private readonly IPasswordRepository _passwordRepository;

    public GetPasswordByIdQueryHandler(IPasswordRepository passwordRepository)
    {
        _passwordRepository = passwordRepository ?? throw new ArgumentNullException(nameof(passwordRepository));
    }

    public async Task<PasswordResponse?> HandleAsync(GetPasswordByIdQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var passwordRecord = await _passwordRepository.FindByIdAsync(query.Id, cancellationToken);

        if (passwordRecord is null)
        {
            return null;
        }

        return new PasswordResponse(passwordRecord.Id, passwordRecord.Value, passwordRecord.CreatedAtUtc);
    }
}
