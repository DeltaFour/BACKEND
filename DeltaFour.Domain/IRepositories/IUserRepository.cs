using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository :  IBaseRepository<Employee>
    {
        Task Create(Employee employee);
        Task Update(Employee employee);
         
    }
}
