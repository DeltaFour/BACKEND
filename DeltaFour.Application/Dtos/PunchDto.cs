using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Dtos
{
    public class PunchDto
    {
        public PunchType Type { get; set; }

        public DateTime TimePunched { get; set; }

        public string? ImageBase64 { get; set; }

        public ShiftType ShiftType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
