using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Dtos
{
    public class AllAttendanceByCompanyResponse
    {
        public Guid AttendanceId { get; set; }
        public String Name { get; set; }

        public DateTime TimePunched { get; set; }

        public Boolean IsLate { get; set; }

        public PunchType Type { get; set; }

        public ShiftType ShiftType { get; set; }
        
        public String? Status { get; set; }

        public String? Justification { get; set; }

        public String? Observation { get; set; }

        public String? FilePath { get; set; }
    }
}
