using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeFaceRepository(AppDbContext context) : IEmployeeFaceRepository
    {
        public async Task<EmployeeFace?> Find(Expression<Func<EmployeeFace, bool>> predicate)
        {
            return await context.EmployeeFaces.FirstOrDefaultAsync(predicate);
        }
        public async Task Create(EmployeeFace employeeFace)
        {
            context.EmployeeFaces.Add(employeeFace);
            await context.SaveChangesAsync();
        }
        public async Task Delete(EmployeeFace employeeFace)
        {
            context.EmployeeFaces.Remove(employeeFace);
            await context.SaveChangesAsync();
        }
        public async Task CreateAll(List<EmployeeFace> employees)
        {
            context.EmployeeFaces.AddRange(employees);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAll(List<EmployeeFace> employees)
        {
            context.EmployeeFaces.RemoveRange(employees);
            await context.SaveChangesAsync();
        }
    }
}