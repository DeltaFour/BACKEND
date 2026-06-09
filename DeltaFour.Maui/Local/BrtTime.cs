using System;

namespace DeltaFour.Maui.Local
{
    public static class BrtTime
    {
        private static readonly Lazy<TimeZoneInfo> ZoneInfo = new(CreateZone);

        public static DateTime Now => ToBrt(DateTime.UtcNow);

        public static DateTime ToBrt(DateTime value)
        {
            var zone = ZoneInfo.Value;
            DateTime brt = value.Kind switch
            {
                DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(value, zone),
                DateTimeKind.Local => TimeZoneInfo.ConvertTime(value, zone),
                DateTimeKind.Unspecified => value,
                _ => TimeZoneInfo.ConvertTime(value, zone)
            };

            return DateTime.SpecifyKind(brt, DateTimeKind.Unspecified);
        }

        public static DateTime ToUtc(DateTime brtValue)
        {
            if (brtValue.Kind == DateTimeKind.Utc)
                return brtValue;

            var brt = ToBrt(brtValue);
            return TimeZoneInfo.ConvertTimeToUtc(brt, ZoneInfo.Value);
        }

        private static TimeZoneInfo CreateZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            }
            catch
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
        }
    }
}
