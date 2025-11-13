using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class EmployeeCreateDto
    {
        [Required]
        public String? Name { get; set; }
        
        [Required]
        public String? RoleName { get; set; }
        
        [Required]
        public String? Email { get; set; }
        
        [Required]
        public String? Password { get; set; }
        
        [Required]
        public String? CellPhone { get; set; }
        
        [Required]
        public List<EmployeeShiftDto> EmployeeShift { get; set; }
        
        [Required]
        public String ImageBase64 { get; set; }
        
        [Required]
        public Boolean IsAllowedBypassCoord { get; set; }
    }
}
