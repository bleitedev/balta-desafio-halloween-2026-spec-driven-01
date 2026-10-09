using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Domain.Entities;

namespace PasswordGenerator.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PasswordRecord> Passwords => Set<PasswordRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordRecord>(builder =>
        {
            builder.ToTable("Passwords");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Value).IsRequired().HasMaxLength(128);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
