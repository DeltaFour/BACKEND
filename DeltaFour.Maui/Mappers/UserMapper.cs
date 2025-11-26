using DeltaFour.Maui.Dto;
using DeltaFour.Maui.Local;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DeltaFour.Maui.Mappers
{
    public static class UserMapper
    {
        public static LocalUser ToLocalUser(ApiUserDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));


            DateTime start = default;
            DateTime end = default;

            bool hasSchedule =
                dto.StartDate != default &&
                !string.IsNullOrWhiteSpace(dto.StartTime) &&
                !string.IsNullOrWhiteSpace(dto.EndTime);

            if (hasSchedule)
            {
                start = CombineDateAndTime(dto.StartDate, dto.StartTime);
                end = CombineDateAndTime(dto.StartDate, dto.EndTime);

                if (end <= start)
                    end = end.AddDays(1);
            }

            return new LocalUser
            {
                Name = dto.Name ?? string.Empty,
                CompanyName = dto.CompanyName ?? string.Empty,
                ShiftType = dto.ShiftType ?? string.Empty,
                StartTime = start,
                EndTime = end,
                ToleranceMinutes = 10,
                RecentActivities = dto.LastsEmployeeAttendances?
                    .Where(a => a != null)
                    .Select(ToRecentActivity)
                    .ToList()
                    ?? new List<RecentActivity>()
            };
        }

        private static RecentActivity ToRecentActivity(ApiEmployeeAttendanceDto src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

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


        /// <summary>
        /// Atualiza as batidas recentes de um usuário já carregado,
        /// usando o payload enxuto de /api/v1/user/refresh-information.
        /// Não mexe em Name, CompanyName, StartTime, EndTime etc.
        /// </summary>
        public static void ApplyRefresh(LocalUser target, ApiUserRefreshDto refresh)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (refresh == null)
                throw new ArgumentNullException(nameof(refresh));

            if (refresh.LastsEmployeeAttendances == null ||
                refresh.LastsEmployeeAttendances.Count == 0)
            {
                return;
            }

            target.RecentActivities ??= new List<RecentActivity>();

            foreach (var src in refresh.LastsEmployeeAttendances)
            {
                if (src == null)
                    continue;

                var act = ToRecentActivity(src);

                bool exists = target.RecentActivities.Any(x =>
                    x.PunchTime == act.PunchTime &&
                    string.Equals(x.PunchType, act.PunchType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.ShiftType, act.ShiftType, StringComparison.OrdinalIgnoreCase));

                if (!exists)
                    target.RecentActivities.Add(act);
            }
        }
    }
}
