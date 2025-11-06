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
        public void Create(EmployeeFace employeeFace)
        {
            context.EmployeeFaces.Add(employeeFace);
        }
        public void Delete(EmployeeFace employeeFace)
        {
            context.EmployeeFaces.Remove(employeeFace);
        }
        public void CreateAll(List<EmployeeFace> employees)
        {
            context.EmployeeFaces.AddRange(employees);
        }
        public void DeleteAll(List<EmployeeFace> employees)
        {
            context.EmployeeFaces.RemoveRange(employees);
        }
    }
}