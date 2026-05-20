namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO com informações do funcionário para a folha de ponto
/// </summary>
public class TimeSheetEmployeeDto
{
    /// <summary>
    /// ID do funcionário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do funcionário
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cargo do funcionário
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Nome do turno de trabalho
    /// </summary>
    public string ShiftName { get; set; } = string.Empty;

    /// <summary>
    /// Horário de início do turno
    /// </summary>
    public TimeOnly ShiftStartTime { get; set; }

    /// <summary>
    /// Horário de término do turno
    /// </summary>
    public TimeOnly ShiftEndTime { get; set; }
}
