using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class UserResponseShiftsDto
    {
        public Guid? Id { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public Boolean IsActive { get; set; }
        
        public ShiftType? WorkShiftType {get; set;}
        
        public TimeOnly? WorkShiftStartTime { get; set; }
        
        public TimeOnly? WorkShiftEndTime { get; set; }
        
        public int? WorkShiftToleranceMinutes { get; set; }
    }
}
