using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class UserPunctualityMetricFactory(AppDbContext dbContext)
{
    private readonly Faker<UserPunctualityMetric> _faker = new Faker<UserPunctualityMetric>("pt_BR")
        .RuleFor(m => m.Id, _ => Guid.NewGuid())
        .RuleFor(m => m.TotalAttendances, f => f.Random.Int(10, 100))
        .RuleFor(m => m.TotalLateAttendances, f => f.Random.Int(0, 20))
        .RuleFor(m => m.LatePercentage, f => f.Random.Double(0, 50))
        .RuleFor(m => m.AverageLateMinutes, f => f.Random.Double(0, 30))
        .RuleFor(m => m.MaxLateMinutes, f => f.Random.Int(0, 60))
        .RuleFor(m => m.TotalAbsences, f => f.Random.Int(0, 10))
        .RuleFor(m => m.TotalWorkedDays, f => f.Random.Int(10, 50))
        .RuleFor(m => m.Cluster, _ => null)
        .RuleFor(m => m.LastCalculatedAt, f => f.Date.Recent())
        .RuleFor(m => m.CreatedAt, f => f.Date.Past());

    public UserPunctualityMetric Generate(Guid userId)
    {
        var metric = _faker.Generate();
        metric.UserId = userId;
        return metric;
    }

    public async Task<UserPunctualityMetric> CreateAsync(Guid userId, Action<UserPunctualityMetric>? configure = null)
    {
        var metric = Generate(userId);
        configure?.Invoke(metric);

        dbContext.UserPunctualityMetrics.Add(metric);
        await dbContext.SaveChangesAsync();

        return metric;
    }

    public async Task<List<UserPunctualityMetric>> CreateAsync(int count, List<Guid> userIds, Action<UserPunctualityMetric>? configure = null)
    {
        var metrics = new List<UserPunctualityMetric>();

        for (int i = 0; i < count && i < userIds.Count; i++)
        {
            var metric = Generate(userIds[i]);
            configure?.Invoke(metric);
            metrics.Add(metric);
        }

        dbContext.UserPunctualityMetrics.AddRange(metrics);
        await dbContext.SaveChangesAsync();

        return metrics;
    }
}
