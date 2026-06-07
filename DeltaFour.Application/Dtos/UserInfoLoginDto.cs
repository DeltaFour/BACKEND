using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Dtos
{
    public class UserInfoLoginDto
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? CompanyName { get; set; }

        public string? Role { get; set; }

        public DateTime? StartDate { get; set; }

        public ShiftType? ShiftType { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public bool IsAllowedBypassCoord { get; set; }

        public bool IsAllowedBypassFace { get; set; }

        public bool MustChangePassword { get; set; }

        public PunchType? LastPunchType { get; set; }

        public List<LastUserAttendancesDto>? LastUserAttendances { get; set; }
    }
}
