using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class LastUserAttendancesDto
    {
        public PunchType? PunchType { get; set; }
        
        public ShiftType? ShiftType { get; set; }
        
        public DateTime? PunchTime { get; set; }
        
        public DateTime? PunchDate { get; set; }
    }
}
