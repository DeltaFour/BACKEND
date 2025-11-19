using DeltaFour.Application.Service;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Infrastructure.Repositories;
using DeltaFour.Infrastructure.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaFour.CrossCutting.Ioc;

public static class MigrationAndSeedersRunner
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<SuperAdminSeeder>();

        await seeder.SeedAsync();
    }
}
