using DeltaFour.API.Filters;
using DeltaFour.Application.Dtos;
using DeltaFour.Application.Services;
using DeltaFour.Infrastructure.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Testcontainers.MySql;

namespace DeltaFour.Test.Features;

public class GlobalExceptionFilterTests : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("deltafour_test")
        .WithUsername("root")
        .WithPassword("root")
        .Build();

    private static readonly string TestSuperAdminId = Guid.NewGuid().ToString();
    private static readonly string TestRoleSuperAdminId = Guid.NewGuid().ToString();

    static GlobalExceptionFilterTests()
    {
        SetupEnvironmentVariables();
        EnsureRsaKeysExist();
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mySqlContainer.StopAsync();
    }

    private WebApplicationFactory<Program> CreateFactoryWithMock(IAuthService mockAuthService)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    var connectionString = _mySqlContainer.GetConnectionString();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                            mySqlOptions => mySqlOptions.UseNetTopologySuite()));

                    var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthService));
                    if (authDescriptor != null)
                        services.Remove(authDescriptor);
                    services.AddScoped(_ => mockAuthService);
                });
                builder.UseEnvironment("Testing");
            });
    }

    [Fact]
    public async Task Login_WhenServiceThrowsInvalidOperationException_ShouldReturnBadRequestErrorResponse()
    {
        var mockAuthService = Substitute.For<IAuthService>();
        mockAuthService.Login(Arg.Any<LoginDto>())
            .ThrowsAsync(new InvalidOperationException("Erro simulado no serviço de autenticação"));

        using var factory = CreateFactoryWithMock(mockAuthService);
        var client = factory.CreateClient();

        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        errorResponse.Message.Should().Be("Erro simulado no serviço de autenticação");
        errorResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Login_WhenServiceThrowsUnauthorizedException_ShouldReturnUnauthorizedErrorResponse()
    {
        var mockAuthService = Substitute.For<IAuthService>();
        mockAuthService.Login(Arg.Any<LoginDto>())
            .ThrowsAsync(new UnauthorizedAccessException("Acesso negado"));

        using var factory = CreateFactoryWithMock(mockAuthService);
        var client = factory.CreateClient();

        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        errorResponse.Message.Should().Be("Acesso não autorizado.");
    }

    [Fact]
    public async Task Login_WhenServiceThrowsKeyNotFoundException_ShouldReturnNotFoundErrorResponse()
    {
        var mockAuthService = Substitute.For<IAuthService>();
        mockAuthService.Login(Arg.Any<LoginDto>())
            .ThrowsAsync(new KeyNotFoundException("Usuário não encontrado"));

        using var factory = CreateFactoryWithMock(mockAuthService);
        var client = factory.CreateClient();

        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        errorResponse.Message.Should().Be("Recurso não encontrado.");
    }

    [Fact]
    public async Task Login_WhenServiceThrowsGenericException_ShouldReturnInternalServerErrorResponse()
    {
        var mockAuthService = Substitute.For<IAuthService>();
        mockAuthService.Login(Arg.Any<LoginDto>())
            .ThrowsAsync(new Exception("Erro genérico inesperado"));

        using var factory = CreateFactoryWithMock(mockAuthService);
        var client = factory.CreateClient();

        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        errorResponse.Message.Should().Be("Ocorreu um erro interno no servidor.");
    }

    [Fact]
    public async Task Login_WhenServiceThrowsArgumentNullException_ShouldReturnBadRequestErrorResponse()
    {
        var mockAuthService = Substitute.For<IAuthService>();
        mockAuthService.Login(Arg.Any<LoginDto>())
            .ThrowsAsync(new ArgumentNullException("email", "O email é obrigatório"));

        using var factory = CreateFactoryWithMock(mockAuthService);
        var client = factory.CreateClient();

        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        errorResponse.Message.Should().Be("Um ou mais parâmetros obrigatórios não foram fornecidos.");
    }

    private static void SetupEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("IS_TESTING", "false");
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
}
