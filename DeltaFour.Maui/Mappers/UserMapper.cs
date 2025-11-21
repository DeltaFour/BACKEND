using DeltaFour.Maui.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DeltaFour.Maui.Local
{
    public static class UserMapper
    {
        public static LocalUser ToLocalUser(ApiUserDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var start = CombineDateAndTime(dto.StartDate, dto.StartTime);
            var end = CombineDateAndTime(dto.StartDate, dto.EndTime);

            if (end <= start)
                end = end.AddDays(1);

            return new LocalUser
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                ShiftType = dto.ShiftType,
                StartTime = start,
                EndTime = end,
                ToleranceMinutes = 10,
                RecentActivities = dto.LastsEmployeeAttendances?
                    .Select(ToRecentActivity)
                    .ToList() ?? new List<RecentActivity>()
            };
        }

        private static RecentActivity ToRecentActivity(ApiEmployeeAttendanceDto src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new RecentActivity
            {
                PunchTime = src.PunchTime,
                PunchType = src.PunchType,
                ShiftType = src.ShiftType
            };
        }

        private static DateTime CombineDateAndTime(DateTime date, string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                throw new FormatException("Horário vazio recebido da API.");

            if (!TimeSpan.TryParse(time, CultureInfo.InvariantCulture, out var timeOfDay))
                throw new FormatException($"Horário em formato inválido: '{time}'.");

            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                timeOfDay.Hours,
                timeOfDay.Minutes,
                timeOfDay.Seconds,
                DateTimeKind.Unspecified);
        }
    }
}
