using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class TreatedUserInformationDto
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Email { get; set; }
        
        public String? RoleName { get; set; }

        public Guid? RoleId { get; set; }

        public Boolean IsAllowedBypassCoord { get; set; }
        
        public Boolean IsActive { get; set; }
        
        public Boolean IsConfirmed { get; set; }
        
        public String Password { get; set; }

        public Guid CompanyId { get; set; }

        public String? CompanyName { get; set; }

        public EmployeeShiftInformationDto? EmployeeShift { get; set; }

        public PunchType? LastPunchType { get; set; }

        public List<LastEmployeeAttendancesDto>? LastsEmployeeAttendances { get; set; }
    }
}
