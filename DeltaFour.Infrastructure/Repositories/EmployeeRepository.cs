using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
    {
        public async Task<List<Employee>> GetAll()
        {
            return await context.Employees.ToListAsync();
        }
        public void Create(Employee employee)
        {
            context.Employees.Add(employee);
        }
        
        public void Update(Employee employee)
        {
            context.Employees.Update(employee);
        }
        public void Delete(Employee employee)
        {
            context.Employees.Remove(employee);
        }

        public async Task<Employee?> Find(Expression<Func<Employee, bool>> predicate)
        {
            return await context
                .Employees
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> Exists(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Employees.AnyAsync(predicate);
        }
    }
}