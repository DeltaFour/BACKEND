using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeRepository :  IBaseRepository<Employee>
    {
        Task<List<Employee>> GetAll();
        Task<bool> Exists(Expression<Func<Employee, bool>> predicate);
        
        void Create(Employee employee);
        
        void Update(Employee employee);
        
        void Delete(Employee employee);
    }
}