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
        public Guid ShiftId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public Boolean IsAllowedBypassCoord { get; set; }
    }
}
