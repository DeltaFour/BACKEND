namespace DeltaFour.Application.Dtos
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }

        public String? Name { get; set; }

        public String? CellPhone { get; set; }

        public Guid? DepartmentId { get; set; }

        public Boolean IsAllowedBypassCoord { get; set; }

        public List<UserShiftDto> UserShift { get; set; }
    }
}
