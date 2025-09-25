using DeltaFour.Application.Service;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaFour.CrossCutting.Ioc;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var isTesting = Environment.GetEnvironmentVariable("IS_TESTING")!;

        if (isTesting.ToLower().Equals("true"))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        }
        else
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAuthRepository, UserAuthRepository>();
            services.AddScoped<AuthService>();
            services.AddScoped<AllRepositories>();
        }

        return services;
    }
}
