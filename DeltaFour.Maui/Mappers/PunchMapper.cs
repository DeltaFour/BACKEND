using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Local;
using System;
using System.Globalization;

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

        public static PunchInDto ToPunchInDto(
            DateTime timeBrt,
            string imageBase64,
            string shiftType,
            string type,
            double latitude,
            double longitude)
        {
            if (string.IsNullOrWhiteSpace(imageBase64))
                throw new ArgumentException("imageBase64 obrigatório.", nameof(imageBase64));

            if (string.IsNullOrWhiteSpace(shiftType))
                throw new ArgumentException("shiftType obrigatório.", nameof(shiftType));
            if (type == "")
            {
                type = "IN";
            }
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type obrigatório.", nameof(type));

            var normalizedType = type.Trim().ToUpperInvariant();

            if (normalizedType is not ("IN" or "OUT"))
                throw new ArgumentException("type deve ser IN ou OUT.", nameof(type));

            var brt = BrtTime.ToBrt(timeBrt);

            var isoBrt = brt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture);

            return new PunchInDto
            {
                Type = normalizedType,
                TimePunched = isoBrt,
                ImageBase64 = imageBase64,
                ShiftType = shiftType,
                Latitude = latitude,
                Longitude = longitude
            };
        }

    }
}
