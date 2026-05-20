namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO principal contendo todos os dados para geração da folha de ponto
/// </summary>
public class TimeSheetDataDto
{
    /// <summary>
    /// Informações da empresa
    /// </summary>
    public TimeSheetCompanyDto Company { get; set; } = new();

    /// <summary>
    /// Informações do funcionário
    /// </summary>
    public TimeSheetEmployeeDto Employee { get; set; } = new();

    /// <summary>
    /// Competência (mês/ano)
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Mês da competência
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Ano da competência
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Lista de dias do mês com registros de ponto
    /// </summary>
    public List<TimeSheetDayDto> Days { get; set; } = new();

    /// <summary>
    /// Totalizadores da folha de ponto
    /// </summary>
    public TimeSheetSummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Data de emissão do relatório
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}
