using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public async Task<Subscription?> Find(Expression<Func<Subscription, bool>> predicate)
    {
        return await context.Subscriptions.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<Subscription>> FindAll(Expression<Func<Subscription, bool>> predicate)
    {
        return await context.Subscriptions.Where(predicate).ToListAsync();
    }

    public void Create(Subscription entity)
    {
        context.Subscriptions.Add(entity);
    }
}
