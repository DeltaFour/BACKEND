using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Action = DeltaFour.Domain.Entities.Action;

namespace DeltaFour.Infrastructure.Repositories
{
    public class ActionRepository(AppDbContext context) : IActionRepository
    {
        public async Task<Action?> Find(Expression<Func<Action, bool>> predicate)
        {
            return await context.Actions.FirstOrDefaultAsync(predicate);  
        }
        
        public async Task<List<Action>> FindAll(Expression<Func<List<Action>, bool>> predicate)
        {
            return await context.Actions.ToListAsync();
        }
    }
}
