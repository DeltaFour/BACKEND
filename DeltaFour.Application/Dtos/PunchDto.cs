using DeltaFour.Domain.Enum;

namespace DeltaFour.Application.Dtos
{
    public class PunchDto
    {
        public PunchType Type { get; set; }

        public DateTime TimePunched { get; set; }

        public String ImageBase64 { get; set; }

        public ShiftType ShiftType { get; set; }

        public Double Latitude { get; set; }

        public Double Longitude { get; set; }


    }
}
