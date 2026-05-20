namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO de requisição para geração da folha de ponto
/// </summary>
public class TimeSheetRequestDto
{
    /// <summary>
    /// ID do funcionário
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Mês da competência (1-12)
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Ano da competência
    /// </summary>
    public int Year { get; set; }
}
