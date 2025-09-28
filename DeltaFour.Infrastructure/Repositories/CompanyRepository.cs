using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class CompanyRepository(AppDbContext context) : ICompanyRepository
    {
        public async Task<Company?> Find(Expression<Func<Company, bool>> predicate)
        {
            return await context.Companies.FindAsync(predicate);
        }

        public async Task<List<Company>> FindAll()
        {
            return context.Companies.ToList();
        }
        public async Task Create(Company company)
        {
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }
        
        public async Task Update(Company company)
        {
            context.Companies.Update(company);
            await context.SaveChangesAsync();
        }
        
        public async Task Delete(Company company)
        {
            context.Companies.Remove(company);
            await context.SaveChangesAsync();
        }
    }
}