using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<User?> GetUserByEmail(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

    }
}
