using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class CompanyGeolocationRepository(AppDbContext context) : ICompanyGeolocationRepository
    {
        public async Task<CompanyGeolocation?> Find(Expression<Func<CompanyGeolocation, bool>> predicate)
        {
            return await context.CompanyGeolocations.FirstOrDefaultAsync(predicate);
        }
        public async Task Create(CompanyGeolocation companyGeolocation)
        {
            context.CompanyGeolocations.Add(companyGeolocation);
            await context.SaveChangesAsync();
        }
    }
}