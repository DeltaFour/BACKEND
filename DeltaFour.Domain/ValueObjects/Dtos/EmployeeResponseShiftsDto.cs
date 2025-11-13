using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class EmployeeResponseShiftsDto
    {
        public Guid? Id { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public ShiftType? WorkShiftType {get; set;}
        
        public DateTime? WorkShiftStartTime { get; set; }
        
        public DateTime? WorkShiftEndTime { get; set; }
        
        public int? WorkShiftToleranceMinutes { get; set; }
    }
}
