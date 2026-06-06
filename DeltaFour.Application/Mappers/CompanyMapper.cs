using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class CompanyMapper
    {
        public static void UpdateCompany(Company company, CompanyGetSettingsDto dto)
        {
            company.LegalName = dto.RazaoSocial!;
            company.Name = dto.NomeFantasia!;
            company.Cnpj = dto.Cnpj;
        }
    }
}
