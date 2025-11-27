using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserFaceRepository(AppDbContext context) : IUserFaceRepository
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
        public void CreateAll(List<UserFace> users)
        {
            context.EmployeeFaces.AddRange(users);
        }
        public void DeleteAll(List<UserFace> users)
        {
            context.EmployeeFaces.RemoveRange(users);
        }
    }
}