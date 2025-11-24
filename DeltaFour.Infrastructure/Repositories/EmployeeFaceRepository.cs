using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeFaceRepository(AppDbContext context) : IEmployeeFaceRepository
    {
        public async Task<UserFace?> Find(Expression<Func<UserFace, bool>> predicate)
        {
            return await context.EmployeeFaces.FirstOrDefaultAsync(predicate);
        }
        public void Create(UserFace userFace)
        {
            context.EmployeeFaces.Add(userFace);
        }
        public void Delete(UserFace userFace)
        {
            context.EmployeeFaces.Remove(userFace);
        }
        public void CreateAll(List<UserFace> employees)
        {
            context.EmployeeFaces.AddRange(employees);
        }
        public void DeleteAll(List<UserFace> employees)
        {
            context.EmployeeFaces.RemoveRange(employees);
        }
    }
}