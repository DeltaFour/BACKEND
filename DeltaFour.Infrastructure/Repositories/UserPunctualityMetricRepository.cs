using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserPunctualityMetricRepository(AppDbContext context) : IUserPunctualityMetricRepository
    {
        public async Task<UserPunctualityMetric?> Find(Expression<Func<UserPunctualityMetric, bool>> predicate)
        {
            return await context.UserPunctualityMetrics.FirstOrDefaultAsync(predicate);
        }

        public void Create(UserPunctualityMetric metric)
        {
            context.UserPunctualityMetrics.Add(metric);
        }

        public void Update(UserPunctualityMetric metric)
        {
            context.UserPunctualityMetrics.Update(metric);
        }

        public async Task<UserPunctualityMetric?> GetByUserId(Guid userId)
        {
            return await context.UserPunctualityMetrics
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<List<UserPunctualityMetric>> GetAllByCompanyId(Guid companyId)
        {
            return await context.UserPunctualityMetrics
                .Include(m => m.User)
                .Where(m => m.User != null && m.User.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<List<UserPunctualityMetric>> GetAll()
        {
            return await context.UserPunctualityMetrics
                .Include(m => m.User)
                .Where(m => m.User != null)
                .ToListAsync();
        }

        public async Task UpdateCluster(Guid userId, int cluster)
        {
            var metric = await context.UserPunctualityMetrics
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (metric != null)
            {
                metric.Cluster = cluster;
                metric.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateClusterBatch(List<(Guid UserId, int Cluster)> updates)
        {
            var userIds = updates.Select(u => u.UserId).ToList();
            var metrics = await context.UserPunctualityMetrics
                .Where(m => userIds.Contains(m.UserId))
                .ToListAsync();

            foreach (var metric in metrics)
            {
                var update = updates.FirstOrDefault(u => u.UserId == metric.UserId);
                metric.Cluster = update.Cluster;
                metric.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
        }
    }
}
