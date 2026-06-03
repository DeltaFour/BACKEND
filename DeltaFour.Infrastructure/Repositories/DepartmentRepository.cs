using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class DepartmentRepository(AppDbContext context) : IDepartmentRepository
    {
        public async Task<Department?> Find(Expression<Func<Department, bool>> predicate)
        {
            return await context.Departments.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TResult>> FindAll<TResult>(Expression<Func<Department, bool>> predicate, Expression<Func<Department, TResult>> selector)
        {
            return await context.Departments
                .Where(predicate)
                .Select(selector)
                .ToListAsync();
        }

        public async Task<bool> HasUsers(Guid departmentId)
        {
            return await context.Employees.AnyAsync(u => u.DepartmentId == departmentId);
        }

        public void Create(Department department)
        {
            context.Departments.Add(department);
        }

        public void Update(Department department)
        {
            context.Departments.Update(department);
        }

        public void Delete(Department department)
        {
            context.Departments.Remove(department);
        }
    }
}
