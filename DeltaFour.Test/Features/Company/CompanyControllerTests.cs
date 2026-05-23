using DeltaFour.Application.Dtos;
using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.Company;

public class CompanyControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;

    public CompanyControllerTests(DeltaFourWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _companyFactory = new CompanyFactory(_dbContext);
        _roleFactory = new RoleFactory(_dbContext);
        _userFactory = new UserFactory(_dbContext);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _scope.Dispose();
    }

    private async Task<string> LoginAsSuperAdminAsync()
    {
        var password = "Test@123";
        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = "SUPER_ADMIN");
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginDto
        {
            Email = user.Email,
            Password = password
        });

        var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");

        return jwt;
    }

    private async Task<string> LoginAsAdminAsync()
    {
        var password = "Test@123";
        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = "ADMIN");
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginDto
        {
            Email = user.Email,
            Password = password
        });

        var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");

        return jwt;
    }

    #region Create Company Tests

    [Fact]
    public async Task Create_WithValidSuperAdmin_ShouldReturnNoContent()
    {
        var jwt = await LoginAsSuperAdminAsync();

        var createRequest = new CreateCompanyRequest
        {
            Name = "New Test Company",
            Cnpj = "12345678901234",
            User = new UserRequest
            {
                Name = "Admin User",
                Email = "admin@newcompany.com",
                Password = "Admin@123"
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin-control/company/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturnUnauthorized()
    {
        var createRequest = new CreateCompanyRequest
        {
            Name = "Test Company",
            Cnpj = "12345678901234"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/admin-control/company/create", createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithNonSuperAdmin_ShouldReturnForbidden()
    {
        var jwt = await LoginAsAdminAsync();

        var createRequest = new CreateCompanyRequest
        {
            Name = "Test Company",
            Cnpj = "12345678901234"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin-control/company/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createRequest);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region ChangeStatus Tests

    [Fact]
    public async Task ChangeStatus_WithValidSuperAdmin_ShouldReturnNoContent()
    {
        var jwt = await LoginAsSuperAdminAsync();
        var company = await _companyFactory.CreateAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/admin-control/company/change-status/{company.Id}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeStatus_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsync($"/api/v1/admin-control/company/change-status/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangeStatus_WithNonSuperAdmin_ShouldReturnForbidden()
    {
        var jwt = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/admin-control/company/change-status/{Guid.NewGuid()}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region List Companies Tests

    [Fact]
    public async Task List_WithValidSuperAdmin_ShouldReturnOk()
    {
        var jwt = await LoginAsSuperAdminAsync();

        await _companyFactory.CreateAsync(3);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin-control/company/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task List_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/admin-control/company/list");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task List_WithNonSuperAdmin_ShouldReturnForbidden()
    {
        var jwt = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin-control/company/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}
