using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeShiftRepository : IBaseRepository<EmployeeShift>
    {
        void Create(EmployeeShift employeeAttendance);
        
        void Update(EmployeeShift employeeAttendance);
        
        void Delete(EmployeeShift employeeAttendance);
    }
}