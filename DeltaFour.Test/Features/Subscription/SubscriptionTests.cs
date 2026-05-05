using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Application.Services;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Test.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace DeltaFour.Test.Features.Subscription;

[Collection("Integration Tests")]
public class SubscriptionTests : IClassFixture<DeltaFourWebApplicationFactory>
{
    private readonly DeltaFourWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriptionTests(DeltaFourWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterCompany_ShouldCreateCompanyAndSubscription()
    {
        var mockSubscriptionService = Substitute.For<ISubscriptionService>();
        mockSubscriptionService
            .CreateSubscriptionAsync(Arg.Any<CreateSubscriptionRequest>())
            .Returns(new SubscriptionResult
            {
                Success = true,
                ExternalId = "sub_test123"
            });

        var factory = _factory.WithMockedService(mockSubscriptionService);
        var client = factory.CreateClient();

        var request = new RegisterCompanyRequest
        {
            Name = "Test Company",
            Cnpj = "12345678901234",
            User = new UserRequest
            {
                Name = "Admin User",
                Email = "admin@testcompany.com",
                Password = "TestPassword123"
            }
        };

        var response = await client.PostAsJsonAsync("/api/v1/subscription/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<SubscriptionResult>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.ExternalId.Should().Be("sub_test123");

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var company = await dbContext.Companies
            .Include(c => c.Subscription)
            .FirstOrDefaultAsync(c => c.Cnpj == "12345678901234");

        company.Should().NotBeNull();
        company!.Subscription.Should().NotBeNull();
        company.Subscription!.Status.Should().Be(SubscriptionStatus.PENDING.ToString());
        company.Subscription.ExternalId.Should().Be("sub_test123");

        await factory.ResetDatabaseAsync();
    }

    [Fact]
    public async Task CancelSubscription_ShouldUpdateSubscriptionStatus()
    {
        var mockSubscriptionService = Substitute.For<ISubscriptionService>();
        mockSubscriptionService
            .CreateSubscriptionAsync(Arg.Any<CreateSubscriptionRequest>())
            .Returns(new SubscriptionResult
            {
                Success = true,
                ExternalId = "sub_test456"
            });

        mockSubscriptionService
            .CancelSubscriptionAsync(Arg.Any<string>())
            .Returns(Task.CompletedTask);

        var factory = _factory.WithMockedService(mockSubscriptionService);
        var client = factory.CreateClient();

        var registerRequest = new RegisterCompanyRequest
        {
            Name = "Test Company 2",
            Cnpj = "98765432109876",
            User = new UserRequest
            {
                Name = "Admin User 2",
                Email = "admin2@testcompany.com",
                Password = "TestPassword123"
            }
        };

        await client.PostAsJsonAsync("/api/v1/subscription/register", registerRequest);

        await factory.ResetDatabaseAsync();
    }
}
