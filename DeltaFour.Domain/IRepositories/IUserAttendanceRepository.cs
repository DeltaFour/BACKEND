using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAttendanceRepository : IBaseRepository<UserAttendance>
    {
        void Create(UserAttendance userAttendance);

        void Update(UserAttendance userAttendance);

        void Delete(UserAttendance userAttendance);

        Task UpdateStatusAttendance(Guid attendanceId, UpdateStatusAttendanceDto dto);

        Task<int> AmountAttendanceIn(Guid userId);
    }
}