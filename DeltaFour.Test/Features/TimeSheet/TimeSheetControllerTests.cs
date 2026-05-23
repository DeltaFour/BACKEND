using DeltaFour.Application.Dtos;
using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.TimeSheet;

public class TimeSheetControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;

    public TimeSheetControllerTests(DeltaFourWebApplicationFactory factory)
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

    private async Task<(string jwt, DeltaFour.Domain.Entities.User user, DeltaFour.Domain.Entities.Company company)> LoginAsAdminAsync()
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

        return (jwt, user, company);
    }

    private async Task<(string jwt, DeltaFour.Domain.Entities.User user)> LoginAsEmployeeAsync(Guid companyId)
    {
        var password = "Test@123";
        var role = await _roleFactory.CreateAsync(companyId, r => r.Name = "EMPLOYEE");
        var user = await _userFactory.CreateWithPasswordAsync(companyId, role.Id, password);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginDto
        {
            Email = user.Email,
            Password = password
        });

        var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");

        return (jwt, user);
    }

    #region List TimeSheets Tests

    [Fact]
    public async Task List_WithValidAuth_ShouldReturnOk()
    {
        var (jwt, _, _) = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/timesheet/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task List_WithFilters_ShouldReturnOk()
    {
        var (jwt, user, _) = await LoginAsAdminAsync();
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/list?userId={user.Id}&month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task List_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/timesheet/list");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Generate PDF Tests

    [Fact]
    public async Task GeneratePdf_WithValidUserAndPeriod_ShouldReturnPdf()
    {
        var (jwt, user, _) = await LoginAsAdminAsync();
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/pdf/{user.Id}?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task GeneratePdf_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync($"/api/v1/timesheet/pdf/{Guid.NewGuid()}?month=1&year=2025");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Generate My PDF Tests

    [Fact]
    public async Task GenerateMyPdf_WithValidAuth_ShouldReturnPdf()
    {
        var (jwt, user, company) = await LoginAsAdminAsync();
        var (employeeJwt, employee) = await LoginAsEmployeeAsync(company.Id);
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/pdf/me?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={employeeJwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
    }

    #endregion

    #region Get TimeSheet Data Tests

    [Fact]
    public async Task GetTimeSheetData_WithValidUserAndPeriod_ShouldReturnData()
    {
        var (jwt, user, _) = await LoginAsAdminAsync();
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/data/{user.Id}?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await response.Content.ReadFromJsonAsync<TimeSheetDataDto>();
        data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTimeSheetData_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync($"/api/v1/timesheet/data/{Guid.NewGuid()}?month=1&year=2025");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get My TimeSheet Data Tests

    [Fact]
    public async Task GetMyTimeSheetData_WithValidAuth_ShouldReturnData()
    {
        var (jwt, _, company) = await LoginAsAdminAsync();
        var (employeeJwt, _) = await LoginAsEmployeeAsync(company.Id);
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/data/me?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={employeeJwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await response.Content.ReadFromJsonAsync<TimeSheetDataDto>();
        data.Should().NotBeNull();
    }

    #endregion

    #region Get Signature Status Tests

    [Fact]
    public async Task GetSignatureStatus_WithValidUserAndPeriod_ShouldReturnStatus()
    {
        var (jwt, user, _) = await LoginAsAdminAsync();
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/status/{user.Id}?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSignatureStatus_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync($"/api/v1/timesheet/status/{Guid.NewGuid()}?month=1&year=2025");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get My Signature Status Tests

    [Fact]
    public async Task GetMySignatureStatus_WithValidAuth_ShouldReturnStatus()
    {
        var (jwt, _, company) = await LoginAsAdminAsync();
        var (employeeJwt, _) = await LoginAsEmployeeAsync(company.Id);
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/v1/timesheet/status/me?month={currentMonth}&year={currentYear}");
        request.Headers.Add("Cookie", $"Jwt={employeeJwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
