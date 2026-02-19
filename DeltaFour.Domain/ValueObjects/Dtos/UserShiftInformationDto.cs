using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class UserShiftInformationDto
    {
        public DateTime? StartDate { get; set; }
        
        public ShiftType? ShiftType { get; set; }
        
        public TimeOnly? StartTime { get; set; }
        
        public TimeOnly? EndTime { get; set; }
    }
}
