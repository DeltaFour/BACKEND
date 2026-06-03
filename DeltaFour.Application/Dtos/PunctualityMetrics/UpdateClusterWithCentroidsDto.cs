namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class UpdateClusterWithCentroidsDto
    {
        public List<UpdateClusterDto> UserClusters { get; set; } = new();

        public List<CentroidDto> Centroids { get; set; } = new();
    }
}
