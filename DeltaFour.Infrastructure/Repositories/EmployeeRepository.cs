using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext context) : IUserRepository
    {
        public async Task<List<Employee>> GetAll()
        {
            return await context.Employees.ToListAsync();
        }
        public async Task Create(Employee employee)
        {
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }
        
        public Task Update(Employee employee)
        {
            context.Employees.Update(employee);
            return context.SaveChangesAsync();
        }
        public async Task Delete(Employee employee)
        {
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
        }

        public async Task<Employee?> Find(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Employees.FirstOrDefaultAsync(predicate);
        }
    }
}
