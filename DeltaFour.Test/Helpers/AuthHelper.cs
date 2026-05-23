using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace DeltaFour.Test.Helpers;

public class AuthHelper
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly Factories.CompanyFactory _companyFactory;
    private readonly Factories.RoleFactory _roleFactory;
    private readonly Factories.UserFactory _userFactory;

    public AuthHelper(HttpClient client, IServiceScope scope, AppDbContext dbContext)
    {
        _client = client;
        _scope = scope;
        _dbContext = dbContext;
        _companyFactory = new Factories.CompanyFactory(_dbContext);
        _roleFactory = new Factories.RoleFactory(_dbContext);
        _userFactory = new Factories.UserFactory(_dbContext);
    }

    public async Task<AuthenticatedUser> CreateAndLoginUserAsync(string roleName = "ADMIN", string password = "Test@123")
    {
        var company = await _companyFactory.CreateAsync();
        var role = await _roleFactory.CreateAsync(company.Id, r => r.Name = roleName);
        var user = await _userFactory.CreateWithPasswordAsync(company.Id, role.Id, password);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var cookies = response.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var refreshCookie = cookies.First(c => c.StartsWith("RefreshToken="));

        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        return new AuthenticatedUser
        {
            User = user,
            Company = company,
            Role = role,
            Jwt = jwt,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthenticatedUser> CreateAndLoginUserAsync(Guid companyId, string roleName = "ADMIN", string password = "Test@123")
    {
        var role = await _roleFactory.CreateAsync(companyId, r => r.Name = roleName);
        var user = await _userFactory.CreateWithPasswordAsync(companyId, role.Id, password);

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var cookies = response.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var refreshCookie = cookies.First(c => c.StartsWith("RefreshToken="));

        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        return new AuthenticatedUser
        {
            User = user,
            Company = null!,
            Role = role,
            Jwt = jwt,
            RefreshToken = refreshToken
        };
    }

    public void SetAuthCookies(HttpClient client, string jwt, string refreshToken)
    {
        client.DefaultRequestHeaders.Add("Cookie", $"Jwt={jwt}; RefreshToken={refreshToken}");
    }
}

public class AuthenticatedUser
{
    public required User User { get; set; }
    public required Company Company { get; set; }
    public required Role Role { get; set; }
    public required string Jwt { get; set; }
    public required string RefreshToken { get; set; }
}
