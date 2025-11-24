using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeAttendanceRepository : IBaseRepository<UserAttendance>
    {
        void Create(UserAttendance userAttendance);
        
        void Update(UserAttendance userAttendance);
        
        void Delete(UserAttendance userAttendance);
    }
}