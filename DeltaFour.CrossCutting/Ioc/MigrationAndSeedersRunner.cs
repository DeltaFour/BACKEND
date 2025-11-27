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
    ///<sumary>
    ///Configuration for migrations and seeder
    ///</sumary>
    public async static Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<SuperAdminSeeder>();
        var seederRole = scope.ServiceProvider.GetRequiredService<RoleSeeder>();

        await seeder.SeedAsync();
        await seederRole.SeedAsync();
    }
}