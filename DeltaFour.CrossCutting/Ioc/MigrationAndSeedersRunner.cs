using DeltaFour.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaFour.CrossCutting.Ioc;

public static class MigrationAndSeedersRunner
{
    ///<summary>
    ///Configuration for migrations and seeder
    ///</summary>
    public async static Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        var isTesting = Environment.GetEnvironmentVariable("IS_TESTING") == "true";

        // Pular migrations em ambiente de teste - o Testcontainer gerencia o banco
        if (isTesting)
        {
            return;
        }

        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
    }
}