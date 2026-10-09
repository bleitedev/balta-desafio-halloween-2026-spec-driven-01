using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Domain.Entities;
using PasswordGenerator.Domain.Interfaces;

namespace PasswordGenerator.Infrastructure.Data;

public sealed class PasswordRepository : IPasswordRepository
{
    private readonly AppDbContext _dbContext;

    public PasswordRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Add(PasswordRecord passwordRecord)
    {
        ArgumentNullException.ThrowIfNull(passwordRecord);
        _dbContext.Passwords.Add(passwordRecord);
    }

    public async Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Passwords.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
    }
}
