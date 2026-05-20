namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO representando um dia na folha de ponto
/// </summary>
public class TimeSheetDayDto
{
    /// <summary>
    /// Data do dia
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Dia da semana por extenso
    /// </summary>
    public string DayOfWeek { get; set; } = string.Empty;

    /// <summary>
    /// Primeiro horário de entrada registrado
    /// </summary>
    public TimeOnly? FirstEntry { get; set; }

    /// <summary>
    /// Último horário de saída registrado
    /// </summary>
    public TimeOnly? LastExit { get; set; }

    /// <summary>
    /// Total de horas trabalhadas no dia
    /// </summary>
    public TimeSpan WorkedHours { get; set; }

    /// <summary>
    /// Total esperado de horas com base no turno
    /// </summary>
    public TimeSpan ExpectedHours { get; set; }

    /// <summary>
    /// Saldo de horas (trabalhadas - esperadas)
    /// </summary>
    public TimeSpan Balance { get; set; }

    /// <summary>
    /// Observações do dia (falta, pendente, incompleto, etc.)
    /// </summary>
    public string Observation { get; set; } = string.Empty;

    /// <summary>
    /// Indica se é dia de folga/DSR
    /// </summary>
    public bool IsDayOff { get; set; }

    /// <summary>
    /// Indica se é um dia futuro (ainda não deve ser contabilizado)
    /// </summary>
    public bool IsFutureDay { get; set; }

    /// <summary>
    /// Indica se o funcionário faltou neste dia
    /// </summary>
    public bool IsAbsent { get; set; }

    /// <summary>
    /// Indica se o registro está incompleto (apenas entrada ou apenas saída)
    /// </summary>
    public bool IsIncomplete { get; set; }
}
