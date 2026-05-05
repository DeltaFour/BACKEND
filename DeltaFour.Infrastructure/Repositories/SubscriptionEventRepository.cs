using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class SubscriptionEventRepository(AppDbContext context) : ISubscriptionEventRepository
{
    public async Task<SubscriptionEvent?> Find(Expression<Func<SubscriptionEvent, bool>> predicate)
    {
        return await context.SubscriptionEvents.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<SubscriptionEvent>> FindAll(Expression<Func<SubscriptionEvent, bool>> predicate)
    {
        return await context.SubscriptionEvents.Where(predicate).ToListAsync();
    }

    public void Create(SubscriptionEvent entity)
    {
        context.SubscriptionEvents.Add(entity);
    }
}
