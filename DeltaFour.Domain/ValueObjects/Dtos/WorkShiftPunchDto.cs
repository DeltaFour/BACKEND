namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class WorkShiftPunchDto
    {
        public TimeOnly StartTime { get; set; }
        
        public TimeOnly EndTime { get; set; }
        
        public int ToleranceMinutes { get; set; }
    }
}
