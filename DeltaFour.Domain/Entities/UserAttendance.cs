using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class UserAttendance : BaseEntity
    {
        public Guid UserId { get; set; }

        public DateTime PunchTime { get; set; }

        public PunchType PunchType { get; set; }

        public ShiftType ShiftType { get; set; }

        public Coordinates Coord { get; set; }

        public bool IsLate { get; set; }

        public TimeOnly? TimeLate { get; set; }

        public string? Justification { get; set; }

        public string? FilePath { get; set; }

        public string? Observation { get; set; }

        public string? Status { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public User? User { get; set; }
    }
}
