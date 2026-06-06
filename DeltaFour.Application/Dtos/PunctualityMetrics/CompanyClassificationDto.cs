namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    /// <summary>
    /// Resultado da classificação K-Means para uma única empresa.
    /// </summary>
    public class CompanyClassificationDto
    {
        public Guid CompanyId { get; set; }

        public List<UpdateClusterDto> UserClusters { get; set; } = new();

        public List<CentroidDto> Centroids { get; set; } = new();
    }
}
