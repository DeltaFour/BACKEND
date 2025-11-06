using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeAttendanceRepository : IBaseRepository<EmployeeAttendance>
    {
        void Create(EmployeeAttendance employeeAttendance);
        
        void Update(EmployeeAttendance employeeAttendance);
        
        void Delete(EmployeeAttendance employeeAttendance);
    }
}