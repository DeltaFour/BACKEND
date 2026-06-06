namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Cellphone { get; set; }

        public string Email { get; set; }

        public string? RoleName { get; set; }

        public string? DepartmentName { get; set; }

        public bool IsActive { get; set; }

        public bool IsAllowedBypassCoord { get; set; }

        public DateTime? LastLogin { get; set; }

        public List<UserResponseShiftsDto>? ShiftDto { get; set; }
    }
}
