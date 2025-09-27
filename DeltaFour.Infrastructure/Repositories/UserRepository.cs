using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task Create(Employee employee)
        {
            context.Users.Add(employee);
            await context.SaveChangesAsync();
        }
        
        public Task Update(Employee employee)
        {
            context.Users.Update(employee);
            return context.SaveChangesAsync();
        }

        public async Task<Employee?> Find(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Users.FirstOrDefaultAsync(predicate);
        }
    }
}
