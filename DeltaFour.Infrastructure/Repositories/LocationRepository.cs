using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class LocationRepository(AppDbContext context) : ILocationRepository
    {
        public async Task<Location?> Find(Expression<Func<Location, bool>> predicate)
        {
            return await context.Locations.FirstOrDefaultAsync(predicate); 
        }

        public async Task<List<Location>> FindAll(Expression<Func<Location, bool>> predicate)
        {
            return await context.Locations.ToListAsync();
        }
    }
}