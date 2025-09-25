using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserAuthRepository(AppDbContext context) : IUserAuthRepository
    {

        public async Task<UserAuth?> Find(Expression<Func<UserAuth, bool>> predicate)
        {
            return await context.Auth.FirstOrDefaultAsync(predicate);
        }

        public async Task Create(UserAuth userAuth)
        {
            context.Auth.Add(userAuth);
            await context.SaveChangesAsync();
        }

        public async Task Delete(UserAuth userAuth)
        {
            context.Auth.Remove(userAuth);
            await context.SaveChangesAsync();

        }
        
        public async Task Update(UserAuth userAuth)
        {
            context.Auth.Update(userAuth);
            await context.SaveChangesAsync();
        }
    }
}
