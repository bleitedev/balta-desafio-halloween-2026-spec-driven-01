namespace PasswordGenerator.Domain.Entities;

public sealed class PasswordRecord
{
    public PasswordRecord(Guid id, string value, DateTimeOffset createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(value);

        Id = id;
        Value = value;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; }

    public string Value { get; }

    public DateTimeOffset CreatedAtUtc { get; }
}