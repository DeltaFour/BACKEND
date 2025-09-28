using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeShiftRepository : IBaseRepository<EmployeeShift>
    {
        Task Create(EmployeeShift employeeAttendance);
        
        Task Update(EmployeeShift employeeAttendance);
        
        Task Delete(EmployeeShift employeeAttendance);
    }
}