using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class EmployeeShiftDto
    {
        [Required]
        public Guid ShiftId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime? EndDate { get; set; }
    }
}
