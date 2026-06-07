using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeltaFour.Infrastructure.Context;

/// <summary>
/// Factory usada apenas em tempo de design (dotnet ef migrations) para construir o
/// modelo sem precisar subir todo o host da API. Usa uma versão fixa do MySQL para
/// não exigir conexão com o banco ao gerar migrations.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? "server=localhost;port=3306;database=deltafour;user=root;password=12345678";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 0)),
            mySqlOptions => mySqlOptions.UseNetTopologySuite());

        return new AppDbContext(optionsBuilder.Options);
    }
}
