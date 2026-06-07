using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        public async Task<Notification?> Find(Expression<Func<Notification, bool>> predicate)
        {
            return await context.Notifications.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Notification>> FindAll(Expression<Func<Notification, bool>> predicate)
        {
            return await context.Notifications.Where(predicate).ToListAsync();
        }

        public void Create(Notification notification)
        {
            context.Notifications.Add(notification);
        }

        public void Update(Notification notification)
        {
            context.Notifications.Update(notification);
        }

        public async Task<List<Notification>> GetRecentByCompany(Guid companyId, DateTime since, int limit)
        {
            return await context.Notifications
                .Include(n => n.User)
                .Where(n => n.CompanyId == companyId && n.CreatedAt >= since)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> MarkAllAsRead(Guid companyId)
        {
            return await context.Notifications
                .Where(n => n.CompanyId == companyId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true));
        }
    }
}
