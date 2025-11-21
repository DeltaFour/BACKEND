using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class EmployeeAttendanceMapper
    {
        public static EmployeeAttendance EmployeeAttendanceFromDto(PunchDto dto, Guid userId)
        {
            return new EmployeeAttendance()
            {
                EmployeeId = userId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(dto.Longitude, dto.Latitude),
                CreatedBy = userId
            };
        }
        
        public static EmployeeAttendance EmployeeAttendanceFromDto(PunchForUserDto dto, Guid userId)
        {
            return new EmployeeAttendance()
            {
                EmployeeId = dto.EmployeeId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(0, 0),
                CreatedBy = userId
            };
        }
    }
}
