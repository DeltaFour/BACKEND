using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<List<Company>> FindAll();
        void Create(Company company);
        void Update(Company company);
        void Delete(Company company);
    }
}
