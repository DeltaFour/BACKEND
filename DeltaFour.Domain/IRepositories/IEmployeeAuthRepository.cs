using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeAuthRepository : IBaseRepository<EmployeeAuth>
    {
        void Create(EmployeeAuth employeeAuth);
        void Update(EmployeeAuth employeeAuth);
        void Delete(EmployeeAuth employeeAuth);
    }
}
