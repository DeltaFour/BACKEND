using DeltaFour.Application.Dtos;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.Auth;

public class LoginTest : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;

    public LoginTest(DeltaFourWebApplicationFactory factory)
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

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithJwtCookie()
    {
        var password = "Test@123";

        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = "ADMIN");
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookieList = cookies!.ToList();

        cookieList.Should().Contain(c => c.StartsWith("Jwt="));
        cookieList.Should().Contain(c => c.StartsWith("RefreshToken="));

        var jwtCookie = cookieList.First(c => c.StartsWith("Jwt="));
        var jwtValue = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        jwtValue.Should().NotBeNullOrEmpty();

        var parts = jwtValue.Split('.');
        parts.Should().HaveCount(3, "JWT should have 3 parts: header.payload.signature");

        var userInfo = await response.Content.ReadFromJsonAsync<UserInfoLoginDto>();
        userInfo.Should().NotBeNull();
        userInfo!.Name.Should().Be(user.Name);
        userInfo.Role.Should().Be("ADMIN");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        var loginDto = new LoginDto
        {
            Email = "nonexistent@deltafour.com",
            Password = "WrongPassword123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ShouldReturnBadRequest()
    {
        var password = "Test@123";

        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = "EMPLOYEE");
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password, u => u.IsActive = false);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithUnconfirmedUser_ShouldReturnBadRequest()
    {
        var password = "Test@123";

        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = "EMPLOYEE");
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password, u => u.IsConfirmed = false);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
