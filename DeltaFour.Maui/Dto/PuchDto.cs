using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Dto
{
    public sealed class PunchDto
    {
        public string TimePunched { get; set; } = "";
        public string PunchType { get; set; } = "";
    }
    public sealed class PunchInDto
    {
        public string Type { get; set; } = "";

        public string TimePunched { get; set; } = "";

        public string ImageBase64 { get; set; } = "";

        public string ShiftType { get; set; } = "";

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
