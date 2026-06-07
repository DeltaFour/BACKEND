using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Dtos
{
    public class AllAttendanceByCompanyResponse
    {
        public Guid AttendanceId { get; set; }

        public string Name { get; set; }

        public DateTime TimePunched { get; set; }

        public bool IsLate { get; set; }

        public PunchType Type { get; set; }

        public ShiftType ShiftType { get; set; }

        public string? Status { get; set; }

        public string? Justification { get; set; }

        public string? Observation { get; set; }

        public string? FilePath { get; set; }
    }
}
