using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Dtos
{
    public class UserInfoLoginDto
    {
        public String Name { get; set; }

        public String? CompanyName { get; set; }

        public string? Role { get; set; }

        public DateTime? StartDate { get; set; }

        public ShiftType? ShiftType { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public PunchType? LastPunchType { get; set; }

        public List<LastUserAttendancesDto>? LastUserAttendances { get; set; }
    }
}
