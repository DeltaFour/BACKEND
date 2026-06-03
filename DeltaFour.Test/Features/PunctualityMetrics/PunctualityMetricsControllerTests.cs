using DeltaFour.Application.Dtos;
using DeltaFour.Application.Dtos.PunctualityMetrics;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Factories;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.PunctualityMetrics;

public class PunctualityMetricsControllerTests : IClassFixture<DeltaFourWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly CompanyFactory _companyFactory;
    private readonly RoleFactory _roleFactory;
    private readonly UserFactory _userFactory;
    private readonly UserAttendanceFactory _attendanceFactory;
    private readonly UserPunctualityMetricFactory _metricFactory;

    public PunctualityMetricsControllerTests(DeltaFourWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _companyFactory = new CompanyFactory(_dbContext);
        _roleFactory = new RoleFactory(_dbContext);
        _userFactory = new UserFactory(_dbContext);
        _attendanceFactory = new UserAttendanceFactory(_dbContext);
        _metricFactory = new UserPunctualityMetricFactory(_dbContext);
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

        loginResponse.EnsureSuccessStatusCode();

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

    #region GetAllByCompany Tests

    [Fact]
    public async Task GetAllByCompany_WithValidAdmin_ShouldReturnMetrics()
    {
        // Arrange
        var (jwt, adminUser, company) = await LoginAsAdminAsync();

        var employee = await _userFactory.CreateAsync(company.Id);
        await _metricFactory.CreateAsync(employee.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var metrics = await response.Content.ReadFromJsonAsync<List<PunctualityMetricResponseDto>>();
        metrics.Should().NotBeNull();
        metrics!.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetAllByCompany_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/punctuality-metrics/list");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllByCompany_WithEmployee_ShouldReturnForbidden()
    {
        // Arrange
        var company = await _companyFactory.CreateAsync();
        var (jwt, _) = await LoginAsEmployeeAsync(company.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/list");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region GetByUserId Tests

    [Fact]
    public async Task GetByUserId_WithValidAdmin_ShouldReturnMetric()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee = await _userFactory.CreateAsync(company.Id);
        var createdMetric = await _metricFactory.CreateAsync(employee.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/punctuality-metrics/{employee.Id}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var metric = await response.Content.ReadFromJsonAsync<PunctualityMetricResponseDto>();
        metric.Should().NotBeNull();
        metric!.UserId.Should().Be(employee.Id);
    }

    [Fact]
    public async Task GetByUserId_WithNonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        var (jwt, _, _) = await LoginAsAdminAsync();
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/punctuality-metrics/{nonExistentUserId}");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetMyMetric Tests

    [Fact]
    public async Task GetMyMetric_WithExistingMetric_ShouldReturnMetric()
    {
        // Arrange
        var company = await _companyFactory.CreateAsync();
        var (jwt, employee) = await LoginAsEmployeeAsync(company.Id);
        await _metricFactory.CreateAsync(employee.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/me");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var metric = await response.Content.ReadFromJsonAsync<PunctualityMetricResponseDto>();
        metric.Should().NotBeNull();
        metric!.UserId.Should().Be(employee.Id);
    }

    [Fact]
    public async Task GetMyMetric_WithNoMetric_ShouldReturnNotFound()
    {
        // Arrange
        var company = await _companyFactory.CreateAsync();
        var (jwt, _) = await LoginAsEmployeeAsync(company.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/me");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Export Tests

    [Fact]
    public async Task ExportForKMeans_WithValidAdmin_ShouldReturnExportData()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee = await _userFactory.CreateAsync(company.Id);
        await _metricFactory.CreateAsync(employee.Id);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/export");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exportData = await response.Content.ReadFromJsonAsync<List<PunctualityMetricExportDto>>();
        exportData.Should().NotBeNull();
        exportData!.Should().ContainSingle();
        exportData.First().UserId.Should().Be(employee.Id);
    }

    #endregion

    #region UpdateCluster Tests

    [Fact]
    public async Task UpdateCluster_WithValidData_ShouldUpdateCluster()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee = await _userFactory.CreateAsync(company.Id);
        await _metricFactory.CreateAsync(employee.Id);

        var updateDto = new UpdateClusterDto
        {
            UserId = employee.Id,
            Cluster = 2
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/punctuality-metrics/cluster");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(updateDto);

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify cluster was updated
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/punctuality-metrics/{employee.Id}");
        getRequest.Headers.Add("Cookie", $"Jwt={jwt}");
        var getResponse = await _client.SendAsync(getRequest);

        var metric = await getResponse.Content.ReadFromJsonAsync<PunctualityMetricResponseDto>();
        metric.Should().NotBeNull();
        metric!.Cluster.Should().Be(2);
    }

    #endregion

    #region UpdateClusterBatch Tests

    [Fact]
    public async Task UpdateClusterBatch_WithValidData_ShouldUpdateAllClusters()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee1 = await _userFactory.CreateAsync(company.Id);
        var employee2 = await _userFactory.CreateAsync(company.Id);
        await _metricFactory.CreateAsync(employee1.Id);
        await _metricFactory.CreateAsync(employee2.Id);

        var batchDto = new UpdateClusterBatchDto
        {
            Updates = new List<UpdateClusterDto>
            {
                new() { UserId = employee1.Id, Cluster = 1 },
                new() { UserId = employee2.Id, Cluster = 2 }
            }
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/punctuality-metrics/cluster/batch");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(batchDto);

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify clusters were updated
        var getRequest1 = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/punctuality-metrics/{employee1.Id}");
        getRequest1.Headers.Add("Cookie", $"Jwt={jwt}");
        var metric1 = await (await _client.SendAsync(getRequest1)).Content.ReadFromJsonAsync<PunctualityMetricResponseDto>();
        metric1!.Cluster.Should().Be(1);

        var getRequest2 = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/punctuality-metrics/{employee2.Id}");
        getRequest2.Headers.Add("Cookie", $"Jwt={jwt}");
        var metric2 = await (await _client.SendAsync(getRequest2)).Content.ReadFromJsonAsync<PunctualityMetricResponseDto>();
        metric2!.Cluster.Should().Be(2);
    }

    #endregion

    #region UpdateClassification Tests

    [Fact]
    public async Task UpdateClassification_WithValidData_ShouldUpdateClustersAndCentroids()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee1 = await _userFactory.CreateAsync(company.Id);
        var employee2 = await _userFactory.CreateAsync(company.Id);
        await _metricFactory.CreateAsync(employee1.Id, m => { m.LatePercentage = 10; m.AverageLateMinutes = 5; });
        await _metricFactory.CreateAsync(employee2.Id, m => { m.LatePercentage = 40; m.AverageLateMinutes = 20; });

        var dto = new UpdateClusterWithCentroidsDto
        {
            UserClusters = new List<UpdateClusterDto>
            {
                new() { UserId = employee1.Id, Cluster = 0 },
                new() { UserId = employee2.Id, Cluster = 1 }
            },
            Centroids = new List<CentroidDto>
            {
                new() { Cluster = 0, LatePercentage = 12, AverageLateMinutes = 6, MaxLateMinutes = 15, TotalAbsences = 1, TotalWorkedDays = 20 },
                new() { Cluster = 1, LatePercentage = 45, AverageLateMinutes = 22, MaxLateMinutes = 60, TotalAbsences = 5, TotalWorkedDays = 20 }
            }
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/punctuality-metrics/classification");
        request.Headers.Add("Cookie", $"Jwt={jwt}");
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify centroids via scatter-plot endpoint
        var scatterRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/scatter-plot");
        scatterRequest.Headers.Add("Cookie", $"Jwt={jwt}");
        var scatterResponse = await _client.SendAsync(scatterRequest);

        scatterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var scatterData = await scatterResponse.Content.ReadFromJsonAsync<ScatterPlotResponseDto>();
        scatterData!.Centroids.Should().HaveCount(2);
    }

    #endregion

    #region GetScatterPlotData Tests

    [Fact]
    public async Task GetScatterPlotData_WithMetricsAndCentroids_ShouldReturnCompleteData()
    {
        // Arrange
        var (jwt, _, company) = await LoginAsAdminAsync();

        var employee1 = await _userFactory.CreateAsync(company.Id, null, u => u.Name = "João Silva");
        var employee2 = await _userFactory.CreateAsync(company.Id, null, u => u.Name = "Maria Santos");
        await _metricFactory.CreateAsync(employee1.Id, m => { m.LatePercentage = 10; m.AverageLateMinutes = 5; m.Cluster = 0; });
        await _metricFactory.CreateAsync(employee2.Id, m => { m.LatePercentage = 40; m.AverageLateMinutes = 20; m.Cluster = 1; });

        // Create centroids
        var classificationDto = new UpdateClusterWithCentroidsDto
        {
            UserClusters = new List<UpdateClusterDto>(),
            Centroids = new List<CentroidDto>
            {
                new() { Cluster = 0, LatePercentage = 12, AverageLateMinutes = 6, MaxLateMinutes = 15, TotalAbsences = 1, TotalWorkedDays = 20 },
                new() { Cluster = 1, LatePercentage = 45, AverageLateMinutes = 22, MaxLateMinutes = 60, TotalAbsences = 5, TotalWorkedDays = 20 }
            }
        };

        var classificationRequest = new HttpRequestMessage(HttpMethod.Put, "/api/v1/punctuality-metrics/classification");
        classificationRequest.Headers.Add("Cookie", $"Jwt={jwt}");
        classificationRequest.Content = JsonContent.Create(classificationDto);
        await _client.SendAsync(classificationRequest);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/scatter-plot");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var scatterData = await response.Content.ReadFromJsonAsync<ScatterPlotResponseDto>();

        scatterData.Should().NotBeNull();
        scatterData!.Points.Should().HaveCount(2);
        scatterData.Centroids.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetScatterPlotData_WithNoData_ShouldReturnEmptyLists()
    {
        // Arrange
        var (jwt, _, _) = await LoginAsAdminAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/punctuality-metrics/scatter-plot");
        request.Headers.Add("Cookie", $"Jwt={jwt}");

        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var scatterData = await response.Content.ReadFromJsonAsync<ScatterPlotResponseDto>();

        scatterData.Should().NotBeNull();
        scatterData!.Points.Should().BeEmpty();
        scatterData.Centroids.Should().BeEmpty();
    }

    #endregion
}
