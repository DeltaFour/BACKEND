using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class WorkShiftDto
    {
        [Required]
        public ShiftType shiftType { get; set; }
        
        [Required]
        public DateTime StartDate  { get; set; }
        
        [Required]
        public DateTime EndDate  { get; set; }
        
        [Required]
        public int ToleranceMinutes  { get; set; }
    }
}
