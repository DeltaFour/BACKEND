namespace DeltaFour.Application.Dtos
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? CellPhone { get; set; }

        public Guid? DepartmentId { get; set; }

        public bool IsAllowedBypassCoord { get; set; }

        public List<UserShiftDto> UserShift { get; set; }
    }
}
