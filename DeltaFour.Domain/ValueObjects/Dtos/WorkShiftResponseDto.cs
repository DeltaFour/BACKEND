using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class WorkShiftResponseDto
    {
        public Guid Id { get; set; }
        
        public ShiftType ShiftType { get; set; }
        
        public TimeOnly StartTime { get; set; }
        
        public TimeOnly EndTime { get; set; }
        
        public int ToleranceMinutes { get; set; }
    }
}
