using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeRepository :  IBaseRepository<Employee>
    {
        Task<List<Employee>> GetAll();
        
        void Create(Employee employee);
        
        void Update(Employee employee);
        
        void Delete(Employee employee);
    }
}