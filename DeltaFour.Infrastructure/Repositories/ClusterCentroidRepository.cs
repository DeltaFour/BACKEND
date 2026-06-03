using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class ClusterCentroidRepository(AppDbContext context) : IClusterCentroidRepository
    {
        public async Task<ClusterCentroid?> Find(Expression<Func<ClusterCentroid, bool>> predicate)
        {
            return await context.ClusterCentroids.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<ClusterCentroid>> FindAll(Expression<Func<ClusterCentroid, bool>> predicate)
        {
            return await context.ClusterCentroids.Where(predicate).ToListAsync();
        }

        public void Create(ClusterCentroid centroid)
        {
            context.ClusterCentroids.Add(centroid);
        }

        public void CreateRange(IEnumerable<ClusterCentroid> centroids)
        {
            context.ClusterCentroids.AddRange(centroids);
        }

        public async Task<List<ClusterCentroid>> GetByCompanyId(Guid companyId)
        {
            return await context.ClusterCentroids
                .Where(c => c.CompanyId == companyId)
                .OrderBy(c => c.Cluster)
                .ToListAsync();
        }

        public async Task DeleteByCompanyId(Guid companyId)
        {
            await context.ClusterCentroids
                .Where(c => c.CompanyId == companyId)
                .ExecuteDeleteAsync();
        }

        public async Task ReplaceForCompany(Guid companyId, List<ClusterCentroid> centroids)
        {
            await DeleteByCompanyId(companyId);

            foreach (var centroid in centroids)
            {
                centroid.CompanyId = companyId;
            }

            context.ClusterCentroids.AddRange(centroids);
            await context.SaveChangesAsync();
        }
    }
}
