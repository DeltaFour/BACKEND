using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class RolePermissionRepository(AppDbContext context) : IRolePermissionRepository
    {
        public async Task<RolePermission?> Find(Expression<Func<RolePermission, bool>> predicate)
        {
            return await context.RolePermissions.FindAsync(predicate);
        }
        
        public async Task Create(RolePermission rolePermission)
        {
            context.RolePermissions.Add(rolePermission);
            await context.SaveChangesAsync();
        }
        
        public async Task Delete(RolePermission rolePermission)
        {
            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteAll(List<RolePermission> rolePermissions)
        {
            context.RolePermissions.RemoveRange(rolePermissions);
            await context.SaveChangesAsync();
        }
    }
}