namespace DeltaFour.Application.Dtos
{
    public class UserCreateDto
    {
        public string? Name { get; set; }

        public string? RoleName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? CellPhone { get; set; }

        public string? Cpf { get; set; }

        public Guid? DepartmentId { get; set; }

        public List<UserShiftDto> UserShift { get; set; }

        public string ImageBase64 { get; set; }

        public bool IsAllowedBypassCoord { get; set; }

        public bool IsAllowedBypassFacial { get; set; }
    }
}
