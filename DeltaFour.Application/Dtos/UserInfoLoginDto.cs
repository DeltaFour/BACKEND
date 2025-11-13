using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Dtos
{
    public class UserInfoLoginDto
    {
        public String Name { get; set; }

        public String? CompanyName { get; set; }

        public DateTime? StartDate { get; set; }

        public ShiftType? ShiftType { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public PunchType? LastPunchType { get; set; }

        public List<LastEmployeeAttendancesDto>? LastsEmployeeAttendances { get; set; }
    }
}
