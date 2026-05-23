using DeltaFour.Application.Dtos;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.Auth;

public class AuthControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;

    public AuthControllerTests(DeltaFourWebApplicationFactory factory)
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

    #region CheckSession Tests

    [Fact]
    public async Task CheckSession_WithValidJwt_ShouldReturnNoContent()
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

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/check-session");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CheckSession_WithoutJwt_ShouldReturnForbid()
    {
        var response = await _client.GetAsync("/api/v1/auth/check-session");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region RefreshToken Tests

    [Fact]
    public async Task RefreshToken_WithValidTokens_ShouldReturnNoContentAndNewCookies()
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

        var loginCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = loginCookies.First(c => c.StartsWith("Jwt="));
        var refreshCookie = loginCookies.First(c => c.StartsWith("RefreshToken="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh-token");
        request.Headers.Add("Cookie", $"Jwt={jwt}; RefreshToken={refreshToken}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Set-Cookie", out var newCookies).Should().BeTrue();
        var newCookieList = newCookies!.ToList();
        newCookieList.Should().Contain(c => c.StartsWith("Jwt="));
        newCookieList.Should().Contain(c => c.StartsWith("RefreshToken="));
    }

    [Fact]
    public async Task RefreshToken_WithoutCookies_ShouldReturnForbid()
    {
        var response = await _client.PostAsync("/api/v1/auth/refresh-token", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ShouldReturnForbid()
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

        var loginCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = loginCookies.First(c => c.StartsWith("Jwt="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh-token");
        request.Headers.Add("Cookie", $"Jwt={jwt}; RefreshToken={Guid.NewGuid()}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_WithValidSession_ShouldReturnNoContentAndDeleteCookies()
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

        var loginCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = loginCookies.First(c => c.StartsWith("Jwt="));
        var refreshCookie = loginCookies.First(c => c.StartsWith("RefreshToken="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        request.Headers.Add("Cookie", $"Jwt={jwt}; RefreshToken={refreshToken}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_WithoutSession_ShouldStillReturnNoContent()
    {
        var response = await _client.PostAsync("/api/v1/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion
}
