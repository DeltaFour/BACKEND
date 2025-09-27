using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAuthRepository : IBaseRepository<EmployeeAuth>
    {
        Task Create(EmployeeAuth employeeAuth);
        Task Update(EmployeeAuth employeeAuth);
        Task Delete(EmployeeAuth employeeAuth);
    }
}
