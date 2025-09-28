using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserAuthRepository(AppDbContext context) : IUserAuthRepository
    {

        public async Task<UserAuth?> FindByUserId(Guid userId)
        {
            return await context.Auth.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<UserAuth?> GetById(Guid id)
        {
            return await context.Auth.FirstOrDefaultAsync(au => au.Id == id);
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
