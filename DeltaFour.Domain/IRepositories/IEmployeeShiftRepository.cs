using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeShiftRepository : IBaseRepository<EmployeeShift>
    {
        void Create(EmployeeShift employeeAttendance);
        
        void CreateAll(List<EmployeeShift> employeeShifts);
        
        void Update(EmployeeShift employeeAttendance);
        
        void Delete(EmployeeShift employeeAttendance);
    }
}