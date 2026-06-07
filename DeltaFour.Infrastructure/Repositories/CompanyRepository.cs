using DeltaFour.Application.Dtos.Responses.Company;
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

        public async Task<List<TResult>> FindAll<TResult>(Expression<Func<Company, TResult>> selector)
        {
            return await context.Companies
                .Select(selector)
                .ToListAsync();
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
        public async Task<CompanyGetSettingsDto?> GetSettings(Guid id)
        {
            return await context.Companies.Where(c => c.Id == id).Select(c => new CompanyGetSettingsDto()
            {
                RazaoSocial = c.LegalName,
                NomeFantasia = c.Name,
                Cnpj = c.Cnpj!,
                Cep = c.Address != null ? c.Address.ZipCode : null,
                Rua = c.Address != null ? c.Address.Street : null,
                Numero = c.Address != null ? c.Address.Number : null,
                Bairro = c.Address != null ? c.Address.District : null,
                Cidade = c.Address != null ? c.Address.City : null,
                Estado = c.Address != null ? c.Address.State : null,
                Complemento = c.Address != null ? c.Address.Complement : null,
                Latitude = c.CompanyGeolocation != null
                    ? c.CompanyGeolocation.Coord.Latitude
                    : null,

                Longitude = c.CompanyGeolocation != null
                    ? c.CompanyGeolocation.Coord.Longitude
                    : null,
                RaioMetros = c.CompanyGeolocation != null ? c.CompanyGeolocation.RadiusMeters : null,
            }).FirstOrDefaultAsync();
        }
        public async Task<Company?> FindWithCoordinates(Guid id)
        {
            return await context.Companies.Where(c => c.Id == id).Include(c => c.CompanyGeolocation)
                .Include(c => c.Address).FirstOrDefaultAsync();
        }
    }
}
