using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class TreatedUserInformationDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? RoleName { get; set; }

        public Guid? RoleId { get; set; }

        public bool IsAllowedBypassCoord { get; set; }

        public bool IsAllowedBypassFace { get; set; }

        public bool IsActive { get; set; }
        public bool IsConfirmed { get; set; }

        public string? Password { get; set; }

        public Guid CompanyId { get; set; }

        public string? CompanyName { get; set; }
        public UserShiftInformationDto? UserShift { get; set; }

        public PunchType? LastPunchType { get; set; }

        public List<LastUserAttendancesDto>? LastsUserAttendances { get; set; }
    }
}
