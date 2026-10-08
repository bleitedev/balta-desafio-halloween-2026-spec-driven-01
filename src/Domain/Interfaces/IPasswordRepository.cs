using PasswordGenerator.Domain.Entities;

namespace PasswordGenerator.Domain.Interfaces;

public interface IPasswordRepository
{
    void Add(PasswordRecord passwordRecord);

    Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}