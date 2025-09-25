using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task Create(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        
        public Task Update(User user)
        {
            context.Users.Update(user);
            return context.SaveChangesAsync();
        }

        public async Task<User?> Find(Expression<Func<User, bool>> predicate)
        {
            return await context.Users.FirstOrDefaultAsync(predicate);
        }
    }
}
