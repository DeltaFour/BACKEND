using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<List<TResult>> FindAll<TResult>(Expression<Func<Company, TResult>> selector);
        void Create(Company company);
        void Update(Company company);
        void Delete(Company company);
    }
}
