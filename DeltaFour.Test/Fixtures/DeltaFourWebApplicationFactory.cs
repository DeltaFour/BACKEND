using DeltaFour.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using Testcontainers.MySql;

namespace DeltaFour.Test.Fixtures;

public class DeltaFourWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("deltafour_test")
        .WithUsername("root")
        .WithPassword("root")
        .Build();

    private static readonly string TestSuperAdminId = Guid.NewGuid().ToString();
    private static readonly string TestRoleSuperAdminId = Guid.NewGuid().ToString();

    private readonly List<Action<IServiceCollection>> _serviceConfigurations = [];

    static DeltaFourWebApplicationFactory()
    {
        SetupEnvironmentVariables();
        EnsureRsaKeysExist();
    }

    public DeltaFourWebApplicationFactory WithMockedService<TService>(TService mock) where TService : class
    {
        _serviceConfigurations.Add(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddScoped(_ => mock);
        });
        return this;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var connectionString = _mySqlContainer.GetConnectionString();

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    mySqlOptions => mySqlOptions.UseNetTopologySuite()));

            foreach (var configuration in _serviceConfigurations)
            {
                configuration(services);
            }
        });

        builder.UseEnvironment("Testing");
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.SubscriptionEvents.ExecuteDeleteAsync();
        await dbContext.Subscriptions.ExecuteDeleteAsync();
        await dbContext.Auth.ExecuteDeleteAsync();
        await dbContext.EmployeeAttendances.ExecuteDeleteAsync();
        await dbContext.EmployeeFaces.ExecuteDeleteAsync();
        await dbContext.EmployeeShifts.ExecuteDeleteAsync();
        await dbContext.Employees.ExecuteDeleteAsync();
        await dbContext.WorkShifts.ExecuteDeleteAsync();
        await dbContext.RolePermissions.ExecuteDeleteAsync();
        await dbContext.Actions.ExecuteDeleteAsync();
        await dbContext.Roles.ExecuteDeleteAsync();
        await dbContext.CompanyGeolocations.ExecuteDeleteAsync();
        await dbContext.Locations.ExecuteDeleteAsync();
        await dbContext.Addresses.ExecuteDeleteAsync();
        await dbContext.Companies.ExecuteDeleteAsync();
    }

    private static void SetupEnvironmentVariables()
    {
        // CONNECTION_STRING temporária para evitar erro no Serilog MySQL sink
        // O DbContext usa a connection string do Testcontainer que é configurada em ConfigureWebHost
        Environment.SetEnvironmentVariable("CONNECTION_STRING", "Server=localhost;Database=test;Uid=root;Pwd=root");
        Environment.SetEnvironmentVariable("IS_TESTING", "true");
        Environment.SetEnvironmentVariable("ALLOWED_HOST", "http://localhost:3000");
        Environment.SetEnvironmentVariable("SUPER_ADMIN_ID", TestSuperAdminId);
        Environment.SetEnvironmentVariable("ROLE_SUPER_ADMIN_ID", TestRoleSuperAdminId);
        Environment.SetEnvironmentVariable("SUPER_ADMIN_EMAIL", "superadmin@test.com");
        Environment.SetEnvironmentVariable("SUPER_ADMIN_PASSWORD", "SuperAdmin@123");
        Environment.SetEnvironmentVariable("SUPER_ADMIN_NAME", "Super Admin Test");
        Environment.SetEnvironmentVariable("SUPER_ADMIN_COMPANY_CNPJ", "00000000000000");
        Environment.SetEnvironmentVariable("SUPER_ADMIN_COMPANY_NAME", "Test Company");
        Environment.SetEnvironmentVariable("VALIDATE_LIFETIME", "false");
        Environment.SetEnvironmentVariable("REQUIRE_EXPIRATION_TIME", "false");
        Environment.SetEnvironmentVariable("VALIDATE_ISSUER_SIGNING_KEY", "true");
        Environment.SetEnvironmentVariable("VALIDATE_ISSUER", "false");
        Environment.SetEnvironmentVariable("VALIDATE_AUDIENCE", "false");
        Environment.SetEnvironmentVariable("FACE_RECOGNITION_BASE_URL", "http://localhost:5000");
        Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", "sk_test_mock");
        Environment.SetEnvironmentVariable("STRIPE_WEBHOOK_SECRET", "whsec_test_mock");
        Environment.SetEnvironmentVariable("STRIPE_PRICE_ID", "price_test_mock");
        Environment.SetEnvironmentVariable("STRIPE_SUCCESS_URL", "http://localhost:3000/success");
        Environment.SetEnvironmentVariable("STRIPE_CANCEL_URL", "http://localhost:3000/cancel");

        // Email configuration for UserService
        Environment.SetEnvironmentVariable("EMAIL_HOST", "smtp.test.com");
        Environment.SetEnvironmentVariable("EMAIL_PORT", "587");
        Environment.SetEnvironmentVariable("EMAIL_USERNAME", "test@test.com");
        Environment.SetEnvironmentVariable("EMAIL_PASSWORD", "testpassword");
        Environment.SetEnvironmentVariable("EMAIL_FROM_EMAIL", "noreply@test.com");
        Environment.SetEnvironmentVariable("EMAIL_FROM_NAME", "DeltaFour Test");
    }

    private static void EnsureRsaKeysExist()
    {
        var privateKeyPath = "../app.key";
        var publicKeyPath = "../app.pub";

        if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
        {
            using var rsa = RSA.Create(2048);

            var privateKey = rsa.ExportRSAPrivateKeyPem();
            var publicKey = rsa.ExportRSAPublicKeyPem();

            File.WriteAllText(privateKeyPath, privateKey);
            File.WriteAllText(publicKeyPath, publicKey);
        }
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
        await EnsureDatabaseCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mySqlContainer.StopAsync();
    }
}
