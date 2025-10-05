using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface ICompanyGeolocationRepository : IBaseRepository<CompanyGeolocation>
    {
        void Create(CompanyGeolocation companyGeolocation);
    }
}
