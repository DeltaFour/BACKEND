using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeAuthRepository(AppDbContext context) : IUserAuthRepository
    {

        public async Task<EmployeeAuth?> Find(Expression<Func<EmployeeAuth, bool>> predicate)
        {
            return await context.Auth.FirstOrDefaultAsync(predicate);
        }

        public async Task Create(EmployeeAuth employeeAuth)
        {
            context.Auth.Add(employeeAuth);
            await context.SaveChangesAsync();
        }

        public async Task Delete(EmployeeAuth employeeAuth)
        {
            context.Auth.Remove(employeeAuth);
            await context.SaveChangesAsync();

        }
        
        public async Task Update(EmployeeAuth employeeAuth)
        {
            context.Auth.Update(employeeAuth);
            await context.SaveChangesAsync();
        }
    }
}
