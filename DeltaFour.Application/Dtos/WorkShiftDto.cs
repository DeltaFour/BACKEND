using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class WorkShiftDto
    {
        [Required]
        public ShiftType ShiftType { get; set; }
        
        [Required]
        public TimeOnly StartTime  { get; set; }
        
        [Required]
        public TimeOnly EndTime  { get; set; }
        
        [Required]
        public int ToleranceMinutes  { get; set; }
    }
}
