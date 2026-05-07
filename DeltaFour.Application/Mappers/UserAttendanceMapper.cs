using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Mappers
{
    public static class UserAttendanceMapper
    {
        ///<summary>
        ///Map information from UserAttendance to UserAttendance
        ///</summary>
        public static UserAttendance UserAttendanceFromDto
            (PunchDto dto, Guid userId, Boolean isLate, TimeOnly? timeLated)
        {
            return new UserAttendance()
            {
                UserId = userId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(dto.Longitude, dto.Latitude),
                CreatedBy = userId,
                IsLate = isLate,
                TimeLate = timeLated,
            };
        }

        public static UserAttendance UserAttendanceFromDto
            (PunchByEmailDto dto, Guid userId, Boolean isLate, TimeOnly? timeLated, String? filePath)
        {
            return new UserAttendance()
            {
                UserId = userId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(0, 0),
                CreatedBy = userId,
                IsLate = isLate,
                TimeLate = timeLated,
                Justification = dto.Justification,
                FilePath = filePath,
                Observation = dto.Observation,
                Status = isLate ? Enum.GetName(StatusAttendance.pendente) :  null
            };
        }

        ///<summary>
        ///Overload from previous method 
        ///</summary>
        public static UserAttendance UserAttendanceFromDto(PunchForUserDto dto, Guid userId)
        {
            return new UserAttendance()
            {
                UserId = dto.UserId,
                PunchTime = dto.TimePunched,
                PunchType = dto.Type,
                ShiftType = dto.ShiftType,
                Coord = new Coordinates(0, 0),
                CreatedBy = userId
            };
        }
    }
}
