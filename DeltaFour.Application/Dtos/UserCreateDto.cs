namespace DeltaFour.Application.Dtos
{
    public class UserCreateDto
    {
        public String? Name { get; set; }

        public String? RoleName { get; set; }

        public String? Email { get; set; }

        public String? Password { get; set; }

        public String? CellPhone { get; set; }

        public List<UserShiftDto> UserShift { get; set; }

        public String ImageBase64 { get; set; }

        public Boolean IsAllowedBypassCoord { get; set; }
        
        public Boolean IsAllowedBypassFacial {get; set;}
    }
}
