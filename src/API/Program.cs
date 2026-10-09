using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Application.Commands;
using PasswordGenerator.Application.Contracts;
using PasswordGenerator.Application.Handlers;
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
builder.Services.AddScoped<GeneratePasswordCommandHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/api/passwords", async (GeneratePasswordCommandHandler handler, CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(new GeneratePasswordCommand(), cancellationToken);

    return Results.Created($"/api/passwords/{response.Id}", response);
});

app.Run();

public partial class Program { }