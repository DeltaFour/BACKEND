using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeAuthRepository(AppDbContext context) : IEmployeeAuthRepository
    {

        public async Task<EmployeeAuth?> Find(Expression<Func<EmployeeAuth, bool>> predicate)
        {
            return await context.Auth.FirstOrDefaultAsync(predicate);
        }

        public void Create(EmployeeAuth employeeAuth)
        {
            context.Auth.Add(employeeAuth);
        }

        public void Delete(EmployeeAuth employeeAuth)
        {
            context.Auth.Remove(employeeAuth);

        }
        
        public void Update(EmployeeAuth employeeAuth)
        {
            context.Auth.Update(employeeAuth);
        }
    }
}
