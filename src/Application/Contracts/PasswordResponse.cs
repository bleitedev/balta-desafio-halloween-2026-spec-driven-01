namespace PasswordGenerator.Application.Contracts;

public sealed record PasswordResponse(
    Guid Id,
    string Password,
    DateTimeOffset CreatedAtUtc);
