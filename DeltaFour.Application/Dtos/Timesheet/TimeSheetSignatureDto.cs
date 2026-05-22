namespace DeltaFour.Application.Dtos.TimeSheet;

/// <summary>
/// DTO com informações de assinatura da folha de ponto
/// </summary>
public class TimeSheetSignatureDto
{
    /// <summary>
    /// Indica se foi assinada pelo funcionário
    /// </summary>
    public bool SignedByEmployee { get; set; }

    /// <summary>
    /// Nome do funcionário para assinatura
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Data/hora da assinatura do funcionário
    /// </summary>
    public DateTime? EmployeeSignedAt { get; set; }

    /// <summary>
    /// Indica se foi assinada pelo RH
    /// </summary>
    public bool SignedByHR { get; set; }

    /// <summary>
    /// Nome do responsável do RH que assinou
    /// </summary>
    public string HRSignerName { get; set; } = string.Empty;

    /// <summary>
    /// Data/hora da assinatura do RH
    /// </summary>
    public DateTime? HRSignedAt { get; set; }
}
