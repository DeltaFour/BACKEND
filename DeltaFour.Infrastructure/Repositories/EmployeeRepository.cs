using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
    {
        public async Task<List<Employee>> GetAll(Guid companyId)
        {
            return await context.Employees.Where(e => e.IsActive == true && e.CompanyId == companyId)
                .Include(e => e.Role)
                .ToListAsync();
        }

        public async Task<bool> FindAny(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Employees.AnyAsync(predicate);
        }
        public async Task<Employee?> FindIncluding(Guid id)
        {
            return await context.Employees.Where(e => e.Id == id)
                .Include(e => e.EmployeeShifts).SingleOrDefaultAsync();
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
            return await context.Employees.FirstOrDefaultAsync(predicate);
        }
    }
}
