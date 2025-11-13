using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class EmployeeShiftInformationDto
    {
        public DateTime? StartDate { get; set; }
        
        public ShiftType? ShiftType { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
    }
}
