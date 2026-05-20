namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO com totalizadores da folha de ponto
/// </summary>
public class TimeSheetSummaryDto
{
    /// <summary>
    /// Total de horas trabalhadas no mês
    /// </summary>
    public TimeSpan TotalWorkedHours { get; set; }

    /// <summary>
    /// Total de horas esperadas no mês
    /// </summary>
    public TimeSpan TotalExpectedHours { get; set; }

    /// <summary>
    /// Total de faltas no mês
    /// </summary>
    public int TotalAbsences { get; set; }

    /// <summary>
    /// Saldo final de horas (trabalhadas - esperadas)
    /// </summary>
    public TimeSpan FinalBalance { get; set; }

    /// <summary>
    /// Indica se o mês está completo
    /// </summary>
    public bool IsMonthComplete { get; set; }
}
