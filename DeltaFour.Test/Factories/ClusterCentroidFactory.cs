using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class ClusterCentroidFactory(AppDbContext dbContext)
{
    private readonly Faker<ClusterCentroid> _faker = new Faker<ClusterCentroid>("pt_BR")
        .RuleFor(c => c.Id, _ => Guid.NewGuid())
        .RuleFor(c => c.LatePercentage, f => f.Random.Double(0, 50))
        .RuleFor(c => c.AverageLateMinutes, f => f.Random.Double(0, 30))
        .RuleFor(c => c.MaxLateMinutes, f => f.Random.Int(0, 60))
        .RuleFor(c => c.TotalAbsences, f => f.Random.Int(0, 10))
        .RuleFor(c => c.TotalWorkedDays, f => f.Random.Int(10, 50))
        .RuleFor(c => c.CalculatedAt, f => f.Date.Recent())
        .RuleFor(c => c.CreatedAt, f => f.Date.Past());

    public ClusterCentroid Generate(Guid companyId, int cluster)
    {
        var centroid = _faker.Generate();
        centroid.CompanyId = companyId;
        centroid.Cluster = cluster;
        return centroid;
    }

    public List<ClusterCentroid> GenerateMany(Guid companyId, int count)
    {
        var centroids = new List<ClusterCentroid>();
        for (int i = 0; i < count; i++)
        {
            centroids.Add(Generate(companyId, i));
        }
        return centroids;
    }

    public async Task<ClusterCentroid> CreateAsync(Guid companyId, int cluster, Action<ClusterCentroid>? configure = null)
    {
        var centroid = Generate(companyId, cluster);
        configure?.Invoke(centroid);

        dbContext.ClusterCentroids.Add(centroid);
        await dbContext.SaveChangesAsync();

        return centroid;
    }

    public async Task<List<ClusterCentroid>> CreateManyAsync(Guid companyId, int count, Action<ClusterCentroid>? configure = null)
    {
        var centroids = GenerateMany(companyId, count);

        foreach (var centroid in centroids)
        {
            configure?.Invoke(centroid);
        }

        dbContext.ClusterCentroids.AddRange(centroids);
        await dbContext.SaveChangesAsync();

        return centroids;
    }
}
