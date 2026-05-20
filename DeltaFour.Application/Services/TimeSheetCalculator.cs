using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using System.Globalization;

namespace DeltaFour.Application.Services;

/// <summary>
/// Classe utilitária para cálculos relacionados à folha de ponto
/// </summary>
public static class TimeSheetCalculator
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    /// <summary>
    /// Nomes dos dias da semana em português
    /// </summary>
    private static readonly Dictionary<DayOfWeek, string> DayOfWeekNames = new()
    {
        { DayOfWeek.Sunday, "Domingo" },
        { DayOfWeek.Monday, "Segunda-feira" },
        { DayOfWeek.Tuesday, "Terça-feira" },
        { DayOfWeek.Wednesday, "Quarta-feira" },
        { DayOfWeek.Thursday, "Quinta-feira" },
        { DayOfWeek.Friday, "Sexta-feira" },
        { DayOfWeek.Saturday, "Sábado" }
    };

    /// <summary>
    /// Nomes dos meses em português
    /// </summary>
    private static readonly Dictionary<int, string> MonthNames = new()
    {
        { 1, "Janeiro" }, { 2, "Fevereiro" }, { 3, "Março" },
        { 4, "Abril" }, { 5, "Maio" }, { 6, "Junho" },
        { 7, "Julho" }, { 8, "Agosto" }, { 9, "Setembro" },
        { 10, "Outubro" }, { 11, "Novembro" }, { 12, "Dezembro" }
    };

    /// <summary>
    /// Obtém o nome do dia da semana em português
    /// </summary>
    public static string GetDayOfWeekName(DayOfWeek dayOfWeek)
    {
        return DayOfWeekNames.TryGetValue(dayOfWeek, out var name) ? name : dayOfWeek.ToString();
    }

    /// <summary>
    /// Obtém o nome do mês em português
    /// </summary>
    public static string GetMonthName(int month)
    {
        return MonthNames.TryGetValue(month, out var name) ? name : month.ToString();
    }

    /// <summary>
    /// Formata a competência (mês/ano)
    /// </summary>
    public static string FormatPeriod(int month, int year)
    {
        return $"{GetMonthName(month)} de {year}";
    }

    /// <summary>
    /// Calcula a duração do turno em horas
    /// </summary>
    public static TimeSpan CalculateShiftDuration(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime > startTime)
        {
            return endTime - startTime;
        }

        // Turno noturno que passa da meia-noite
        return TimeSpan.FromHours(24) - (startTime - endTime);
    }

    /// <summary>
    /// Calcula as horas trabalhadas no dia
    /// </summary>
    public static TimeSpan CalculateWorkedHours(TimeOnly? firstEntry, TimeOnly? lastExit)
    {
        if (!firstEntry.HasValue || !lastExit.HasValue)
        {
            return TimeSpan.Zero;
        }

        if (lastExit.Value > firstEntry.Value)
        {
            return lastExit.Value - firstEntry.Value;
        }

        // Caso o último horário seja no dia seguinte (turno noturno)
        return TimeSpan.FromHours(24) - (firstEntry.Value - lastExit.Value);
    }

    /// <summary>
    /// Verifica se o dia é um dia útil (segunda a sexta)
    /// </summary>
    public static bool IsWorkday(DayOfWeek dayOfWeek)
    {
        return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
    }

    /// <summary>
    /// Verifica se o status do registro é aprovado
    /// </summary>
    public static bool IsApproved(string? status)
    {
        return string.Equals(status, StatusAttendance.aprovado.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Filtra e agrupa registros de ponto por dia, considerando apenas aprovados
    /// </summary>
    public static Dictionary<DateOnly, List<UserAttendance>> GroupAttendancesByDay(
        IEnumerable<UserAttendance> attendances)
    {
        return attendances
            .Where(a => IsApproved(a.Status))
            .GroupBy(a => DateOnly.FromDateTime(a.PunchTime))
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.PunchTime).ToList());
    }

    /// <summary>
    /// Obtém o primeiro horário de entrada do dia
    /// </summary>
    public static TimeOnly? GetFirstEntry(List<UserAttendance> dayAttendances)
    {
        var firstIn = dayAttendances
            .Where(a => a.PunchType == PunchType.IN)
            .OrderBy(a => a.PunchTime)
            .FirstOrDefault();

        return firstIn != null ? TimeOnly.FromDateTime(firstIn.PunchTime) : null;
    }

    /// <summary>
    /// Obtém o último horário de saída do dia
    /// </summary>
    public static TimeOnly? GetLastExit(List<UserAttendance> dayAttendances)
    {
        var lastOut = dayAttendances
            .Where(a => a.PunchType == PunchType.OUT)
            .OrderByDescending(a => a.PunchTime)
            .FirstOrDefault();

        return lastOut != null ? TimeOnly.FromDateTime(lastOut.PunchTime) : null;
    }

    /// <summary>
    /// Determina a observação do dia
    /// </summary>
    public static string GetDayObservation(
        bool isDayOff, 
        bool isFutureDay, 
        bool hasApprovedRecords, 
        TimeOnly? firstEntry, 
        TimeOnly? lastExit)
    {
        if (isFutureDay)
        {
            return string.Empty;
        }

        if (isDayOff)
        {
            return "Folga/DSR";
        }

        if (!hasApprovedRecords)
        {
            return "Falta";
        }

        if (!firstEntry.HasValue || !lastExit.HasValue)
        {
            return "Incompleto";
        }

        return string.Empty;
    }

    /// <summary>
    /// Formata um TimeSpan como HH:MM
    /// </summary>
    public static string FormatTimeSpan(TimeSpan time)
    {
        var totalHours = (int)Math.Abs(time.TotalHours);
        var minutes = Math.Abs(time.Minutes);
        var sign = time < TimeSpan.Zero ? "-" : "";
        return $"{sign}{totalHours:D2}:{minutes:D2}";
    }

    /// <summary>
    /// Formata um TimeOnly como HH:MM
    /// </summary>
    public static string FormatTimeOnly(TimeOnly? time)
    {
        return time?.ToString("HH:mm") ?? "--:--";
    }

    /// <summary>
    /// Obtém o nome do tipo de turno
    /// </summary>
    public static string GetShiftTypeName(ShiftType shiftType)
    {
        return shiftType switch
        {
            ShiftType.Matutino => "Matutino",
            ShiftType.Diurno => "Diurno",
            ShiftType.Noturno => "Noturno",
            _ => shiftType.ToString()
        };
    }

    /// <summary>
    /// Calcula os totalizadores da folha de ponto
    /// </summary>
    public static TimeSheetSummaryDto CalculateSummary(List<TimeSheetDayDto> days, bool isMonthComplete)
    {
        var relevantDays = days.Where(d => !d.IsFutureDay).ToList();

        var totalWorked = relevantDays.Aggregate(
            TimeSpan.Zero, 
            (sum, day) => sum + day.WorkedHours);

        var totalExpected = relevantDays
            .Where(d => !d.IsDayOff)
            .Aggregate(TimeSpan.Zero, (sum, day) => sum + day.ExpectedHours);

        var totalAbsences = relevantDays.Count(d => d.IsAbsent && !d.IsDayOff);

        return new TimeSheetSummaryDto
        {
            TotalWorkedHours = totalWorked,
            TotalExpectedHours = totalExpected,
            TotalAbsences = totalAbsences,
            FinalBalance = totalWorked - totalExpected,
            IsMonthComplete = isMonthComplete
        };
    }

    /// <summary>
    /// Formata o endereço completo
    /// </summary>
    public static string FormatFullAddress(Address? address)
    {
        if (address == null)
        {
            return "Endereço não cadastrado";
        }

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(address.Street))
        {
            parts.Add(address.Street);
        }

        if (address.Number.HasValue)
        {
            parts.Add($"nº {address.Number}");
        }

        if (!string.IsNullOrWhiteSpace(address.District))
        {
            parts.Add(address.District);
        }

        if (!string.IsNullOrWhiteSpace(address.City))
        {
            var cityState = address.City;
            if (!string.IsNullOrWhiteSpace(address.State))
            {
                cityState += $" - {address.State}";
            }
            parts.Add(cityState);
        }

        if (!string.IsNullOrWhiteSpace(address.ZipCode))
        {
            parts.Add($"CEP: {address.ZipCode}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Endereço não cadastrado";
    }

    /// <summary>
    /// Formata CNPJ com máscara
    /// </summary>
    public static string FormatCnpj(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            return "Não informado";
        }

        var numbers = new string(cnpj.Where(char.IsDigit).ToArray());

        if (numbers.Length != 14)
        {
            return cnpj;
        }

        return $"{numbers[..2]}.{numbers[2..5]}.{numbers[5..8]}/{numbers[8..12]}-{numbers[12..]}";
    }
}
