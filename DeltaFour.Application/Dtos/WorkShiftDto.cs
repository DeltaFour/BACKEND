using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Dtos
{
    public class WorkShiftDto
    {
        public ShiftType ShiftType { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int ToleranceMinutes { get; set; }
    }
}
