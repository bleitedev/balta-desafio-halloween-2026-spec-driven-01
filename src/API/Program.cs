using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Domain.Interfaces;
using PasswordGenerator.Domain.Services;
using PasswordGenerator.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=passwords.db");
});

builder.Services.AddSingleton<PasswordGeneratorService>();
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();

public partial class Program { }