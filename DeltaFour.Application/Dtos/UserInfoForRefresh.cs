using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Dtos
{
    public class UserInfoForRefresh
    {
        public PunchType? LastPunchType { get; set; }

        public List<LastEmployeeAttendancesDto>? LastsEmployeeAttendances { get; set; }
    }
}
