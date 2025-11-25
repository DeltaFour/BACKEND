using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class EmployeeAttendanceMapper
    {
        public static UserAttendance EmployeeAttendanceFromDto(PunchDto dto, Guid userId, Boolean isLate, TimeOnly? timeLated)
        {
            return new UserAttendance()
            {
                EmployeeId = userId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(dto.Longitude, dto.Latitude),
                CreatedBy = userId,
                IsLate = isLate,
            };
        }
        
        public static UserAttendance EmployeeAttendanceFromDto(PunchForUserDto dto, Guid userId)
        {
            return new UserAttendance()
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
