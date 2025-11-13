using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class EmployeeShiftDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        public Guid ShiftId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }
}
