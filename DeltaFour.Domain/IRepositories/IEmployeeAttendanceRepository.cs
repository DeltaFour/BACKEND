using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeAttendanceRepository : IBaseRepository<EmployeeAttendance>
    {
        Task Create(EmployeeAttendance employeeAttendance);
        
        Task Update(EmployeeAttendance employeeAttendance);
        
        Task Delete(EmployeeAttendance employeeAttendance);
    }
}