namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO com informações da empresa para a folha de ponto
/// </summary>
public class TimeSheetCompanyDto
{
    /// <summary>
    /// Nome da empresa
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da empresa
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Endereço completo formatado
    /// </summary>
    public string FullAddress { get; set; } = string.Empty;
}
