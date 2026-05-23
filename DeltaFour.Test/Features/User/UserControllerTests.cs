using DeltaFour.Application.Dtos;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.User;

public class UserControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;
    private readonly WorkShiftFactory _workShiftFactory;

    public UserControllerTests(DeltaFourWebApplicationFactory factory)
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

    private async Task<(string jwt, string refreshToken, DeltaFour.Domain.Entities.User user, DeltaFour.Domain.Entities.Company company)> LoginAsAdminAsync()
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

        loginResponse.EnsureSuccessStatusCode();

        var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var jwtCookie = cookies.First(c => c.StartsWith("Jwt="));
        var refreshCookie = cookies.First(c => c.StartsWith("RefreshToken="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        return (jwt, refreshToken, user, company);
    }

    private async Task<(string jwt, string refreshToken, DeltaFour.Domain.Entities.User user)> LoginAsEmployeeAsync(Guid companyId)
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
        var refreshCookie = cookies.First(c => c.StartsWith("RefreshToken="));
        var jwt = jwtCookie.Split(';')[0].Replace("Jwt=", "");
        var refreshToken = refreshCookie.Split(';')[0].Replace("RefreshToken=", "");

        return (jwt, refreshToken, user);
    }

    #region GetAllByCompany Tests

    [Fact]
    public async Task GetAllByCompany_WithValidAdmin_ShouldReturnUsers()
    {
        var (jwt, _, _, company) = await LoginAsAdminAsync();

        await _userFactory.CreateAsync(3, company.Id);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetAllByCompany_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/user/list");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Create User Tests

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnOk()
    {
        var (jwt, _, _, company) = await LoginAsAdminAsync();
        var workShift = await _workShiftFactory.CreateAsync(company.Id);

        // Imagem PNG 1x1 pixel válida em base64
        var validImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";

        var createDto = new UserCreateDto
        {
            Name = "New Test User",
            Email = "newuser@test.com",
            Password = "NewUser@123",
            RoleName = "EMPLOYEE",
            CellPhone = "11999999999",
            ImageBase64 = validImageBase64,
            IsAllowedBypassCoord = false,
            IsAllowedBypassFacial = false,
            UserShift = new List<UserShiftDto>
            {
                new UserShiftDto
                {
                    ShiftId = workShift.Id,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/user/create");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(createDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUser_WithoutAuth_ShouldReturnUnauthorized()
    {
        var createDto = new UserCreateDto
        {
            Name = "Test User",
            Email = "test@test.com",
            Password = "Test@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/user/create", createDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Update User Tests

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnOk()
    {
        var (jwt, _, _, company) = await LoginAsAdminAsync();
        var userToUpdate = await _userFactory.CreateAsync(company.Id);

        var updateDto = new UserUpdateDto
        {
            Id = userToUpdate.Id,
            Name = "Updated Name",
            CellPhone = "11888888888",
            IsAllowedBypassCoord = true,
            UserShift = new List<UserShiftDto>()
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/v1/user/update");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(updateDto);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region ChangeStatus Tests

    [Fact]
    public async Task ChangeStatus_WithValidUserId_ShouldReturnOk()
    {
        var (jwt, _, _, company) = await LoginAsAdminAsync();
        var userToDeactivate = await _userFactory.CreateAsync(company.Id);

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/user/change-status/{userToDeactivate.Id}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region RefreshInformation Tests

    [Fact]
    public async Task RefreshInformation_WithValidSession_ShouldReturnUserInfo()
    {
        var (jwt, _, _, company) = await LoginAsAdminAsync();
        var (employeeJwt, _, employee) = await LoginAsEmployeeAsync(company.Id);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user/refresh-information");
        request.Headers.Add("Cookie", $"Jwt={employeeJwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userInfo = await response.Content.ReadFromJsonAsync<UserInfoLoginDto>();
        userInfo.Should().NotBeNull();
        userInfo!.Name.Should().Be(employee.Name);
    }

    #endregion

    #region GetAllAttendancesByCompany Tests

    [Fact]
    public async Task GetAllAttendancesByCompany_WithValidAdmin_ShouldReturnOk()
    {
        var (jwt, _, _, _) = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user/get-all-attendances");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GetAllSelect Tests

    [Fact]
    public async Task GetAllSelect_WithValidAuth_ShouldReturnUsers()
    {
        var (jwt, _, _, _) = await LoginAsAdminAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user/get-all-select");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
