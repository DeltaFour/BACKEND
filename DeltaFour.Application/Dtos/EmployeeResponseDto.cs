namespace DeltaFour.Application.Dtos
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String? Cellphone { get; set; }
        
        public String Email { get; set; }

        public String RoleName { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsAllowedBypassCoord { get; set; }

        public DateTime? LastLogin { get; set; }

        public EmployeeResponseDto
        (
            Guid id,
            String name,
            String email,
            String? cellphone,
            String? roleName,
            Boolean isActive,
            Boolean isAllowedBypassCoord,
            DateTime? lastLogin
        )
        {
            Id = id;
            Name = name;
            Email = email;
            Cellphone = cellphone;
            RoleName = roleName;
            IsActive = isActive;
            IsAllowedBypassCoord = isAllowedBypassCoord;
            LastLogin = lastLogin;
        }
    }
}
