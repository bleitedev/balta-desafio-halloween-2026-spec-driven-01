using PasswordGenerator.Application.Commands;
using PasswordGenerator.Application.Contracts;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Domain.Interfaces;
using PasswordGenerator.Domain.Services;

namespace PasswordGenerator.Application.Handlers;

public sealed class GeneratePasswordCommandHandler
{
    private readonly PasswordGeneratorService _passwordGeneratorService;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GeneratePasswordCommandHandler(
        PasswordGeneratorService passwordGeneratorService,
        IPasswordRepository passwordRepository,
        IUnitOfWork unitOfWork)
    {
        _passwordGeneratorService = passwordGeneratorService ?? throw new ArgumentNullException(nameof(passwordGeneratorService));
        _passwordRepository = passwordRepository ?? throw new ArgumentNullException(nameof(passwordRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PasswordResponse> HandleAsync(GeneratePasswordCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        string password = _passwordGeneratorService.GeneratePassword();
        Guid id = Guid.NewGuid();
        DateTimeOffset createdAtUtc = DateTimeOffset.UtcNow;

        var passwordRecord = new PasswordRecord(id, password, createdAtUtc);
        _passwordRepository.Add(passwordRecord);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new PasswordResponse(id, password, createdAtUtc);
    }
}
