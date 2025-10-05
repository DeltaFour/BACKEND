using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class CompanyRepository(AppDbContext context) : ICompanyRepository
    {
        public async Task<Company?> Find(Expression<Func<Company, bool>> predicate)
        {
            return await context.Companies.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Company>> FindAll()
        {
            return await context.Companies.ToListAsync();
        }
        
        public void Create(Company company)
        {
            context.Companies.Add(company);
        }
        
        public void Update(Company company)
        {
            context.Companies.Update(company);
        }
        
        public void Delete(Company company)
        {
            context.Companies.Remove(company);
        }
    }
}