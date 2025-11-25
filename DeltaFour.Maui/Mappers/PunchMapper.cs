using DeltaFour.Maui.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Mappers
{
    public static class PunchMapper
    {
        public static PunchDto FromBrtTime(DateTime timeBrt, bool punchingOut)
        {
            var punchType = punchingOut ? "OUT" : "IN";

            return new PunchDto
            {
                TimePunched = timeBrt.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                PunchType = punchType
            };
        }
    }
}
