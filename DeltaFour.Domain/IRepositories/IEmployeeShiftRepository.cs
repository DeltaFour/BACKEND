using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeShiftRepository : IBaseRepository<EmployeeShift>
    {
        void Create(EmployeeShift employeeAttendance);
        
        void CreateAll(List<EmployeeShift> employeeShifts);
        
        void Update(EmployeeShift employeeAttendance);
        
        void Delete(EmployeeShift employeeAttendance);

        Task<Boolean> FindAny(Expression<Func<EmployeeShift, bool>> predicate);
        
        void UpdateAll(List<EmployeeShift> employeeShifts);
        
        void DeleteAll(List<EmployeeShift> employeeShifts);
    }
}