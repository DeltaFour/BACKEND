namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    /// <summary>
    /// Payload de classificação K-Means agregado por empresa, enviado pela API Python.
    /// </summary>
    public class UpdateClassificationAllCompaniesDto
    {
        public List<CompanyClassificationDto> Companies { get; set; } = new();
    }
}
