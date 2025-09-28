using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        public async Task<Role?> Find(Expression<Func<Role, bool>> predicate)
        {
            return await context.Roles.FirstOrDefaultAsync(predicate);
        }
        
        public async Task Create(Role role)
        {
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }
        
        public async Task Update(Role role)
        {
            context.Roles.Update(role);
            await context.SaveChangesAsync();
        }
        
        public async Task Delete(Role role)
        {
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
        }
    }
}