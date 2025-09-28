using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository :  IBaseRepository<Employee>
    {
        Task<List<Employee>> GetAll();
        
        Task Create(Employee employee);
        
        Task Update(Employee employee);
        
        Task Delete(Employee employee);
    }
}