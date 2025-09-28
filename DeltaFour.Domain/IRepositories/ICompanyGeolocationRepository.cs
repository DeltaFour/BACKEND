using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface ICompanyGeolocationRepository : IBaseRepository<CompanyGeolocation>
    {
        Task Create(CompanyGeolocation companyGeolocation);
    }
}
