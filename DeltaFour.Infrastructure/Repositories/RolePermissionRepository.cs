using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class RolePermissionRepository(AppDbContext context) : IRolePermissionRepository
    {
        public async Task<RolePermission?> Find(Expression<Func<RolePermission, bool>> predicate)
        {
            return await context.RolePermissions.FirstOrDefaultAsync(predicate);
        }
        
        public void Create(RolePermission rolePermission)
        {
            context.RolePermissions.Add(rolePermission);
        }
        
        public void Delete(RolePermission rolePermission)
        {
            context.RolePermissions.Remove(rolePermission);
        }
        
        public void DeleteAll(List<RolePermission> rolePermissions)
        {
            context.RolePermissions.RemoveRange(rolePermissions);
        }
    }
}