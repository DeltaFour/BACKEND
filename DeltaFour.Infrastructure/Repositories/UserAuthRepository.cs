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

        public void Create(UserAuth userAuth)
        {
            context.Auth.Add(userAuth);
        }

        public void Delete(UserAuth userAuth)
        {
            context.Auth.Remove(userAuth);

        }
        
        public void Update(UserAuth userAuth)
        {
            context.Auth.Update(userAuth);
        }
    }
}
