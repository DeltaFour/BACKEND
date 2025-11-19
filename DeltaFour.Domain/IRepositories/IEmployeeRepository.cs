using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using System.Linq.Expressions;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeRepository :  IBaseRepository<Employee>
    {
        Task<List<Employee>> GetAll();
        
        Task<bool> Exists(Expression<Func<Employee, bool>> predicate);
        
        Task<List<EmployeeResponseDto>> GetAll(Guid companyId);
        
        Task<Boolean> FindAny(Expression<Func<Employee, bool>> predicate);
        
        Task<Employee?> FindIncluding(Guid id);

        Task<Employee?> FindForPunchIn(Guid id);
        
        void Create(Employee employee);

        Task<TreatedUserInformationDto?> FindUserInformation(String email, TimeOnly date);
        
        void Update(Employee employee);

    }
}