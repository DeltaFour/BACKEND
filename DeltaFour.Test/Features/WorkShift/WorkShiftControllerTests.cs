using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.WorkShift;

public class WorkShiftControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;
    private readonly WorkShiftFactory _workShiftFactory;

    public WorkShiftControllerTests(DeltaFourWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _companyFactory = new CompanyFactory(_dbContext);
        _roleFactory = new RoleFactory(_dbContext);
        _userFactory = new UserFactory(_dbContext);
        _workShiftFactory = new WorkShiftFactory(_dbContext);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _scope.Dispose();
    }

    private async Task<(string jwt, DeltaFour.Domain.Entities.Company company)> LoginAsAdminAsync()
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

        return (jwt, company);
    }

    #region List WorkShifts Tests

    [Fact]
    public async Task List_WithValidAuth_ShouldReturnWorkShifts()
    {
        var (jwt, company) = await LoginAsAdminAsync();

        await _workShiftFactory.CreateAsync(3, company.Id);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/workshift/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
        var workShifts = await response.Content.ReadFromJsonAsync<List<WorkShiftResponseDto>>(options);
        workShifts.Should().NotBeNull();
        workShifts!.Count.Should().BeGreaterOrEqualTo(3);
    }

    [Fact]
    public async Task List_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/workshift/list");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task List_WithNoWorkShifts_ShouldReturnEmptyList()
    {
        var (jwt, _) = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/workshift/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workShifts = await response.Content.ReadFromJsonAsync<List<WorkShiftResponseDto>>();
        workShifts.Should().NotBeNull();
    }

    #endregion

    #region Create WorkShift Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnOk()
    {
        var (jwt, _) = await LoginAsAdminAsync();

        var createDto = new WorkShiftDto
        {
            ShiftType = ShiftType.Matutino,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            ToleranceMinutes = 10
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/workshift/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturnUnauthorized()
    {
        var createDto = new WorkShiftDto
        {
            ShiftType = ShiftType.Matutino,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            ToleranceMinutes = 10
        };

        var response = await _client.PostAsJsonAsync("/api/v1/workshift/create", createDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithDiurnoShift_ShouldReturnOk()
    {
        var (jwt, _) = await LoginAsAdminAsync();

        var createDto = new WorkShiftDto
        {
            ShiftType = ShiftType.Diurno,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(17, 0),
            ToleranceMinutes = 15
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/workshift/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithNoturnoShift_ShouldReturnOk()
    {
        var (jwt, _) = await LoginAsAdminAsync();

        var createDto = new WorkShiftDto
        {
            ShiftType = ShiftType.Noturno,
            StartTime = new TimeOnly(22, 0),
            EndTime = new TimeOnly(6, 0),
            ToleranceMinutes = 10
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/workshift/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Update WorkShift Tests

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        var (jwt, company) = await LoginAsAdminAsync();
        var workShift = await _workShiftFactory.CreateAsync(company.Id);

        var updateDto = new WorkShiftUpdateDto
        {
            Id = workShift.Id,
            ShiftType = ShiftType.Diurno,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            ToleranceMinutes = 20
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/v1/workshift/update");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(updateDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WithoutAuth_ShouldReturnUnauthorized()
    {
        var updateDto = new WorkShiftUpdateDto
        {
            Id = Guid.NewGuid(),
            ShiftType = ShiftType.Diurno,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            ToleranceMinutes = 20
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/workshift/update", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region ChangeStatus (Delete) WorkShift Tests

    [Fact]
    public async Task ChangeStatus_WithValidWorkShiftId_ShouldReturnOk()
    {
        var (jwt, company) = await LoginAsAdminAsync();
        var workShift = await _workShiftFactory.CreateAsync(company.Id);

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/workshift/change-status/{workShift.Id}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangeStatus_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync($"/api/v1/workshift/change-status/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
