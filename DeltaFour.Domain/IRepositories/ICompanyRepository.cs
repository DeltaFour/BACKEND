using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<List<Company>> FindAll();
        Task Create(Company company);
        Task Update(Company company);
        Task Delete(Company company);
    }
}
