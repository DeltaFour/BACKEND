namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class UpdateClusterBatchDto
    {
        public List<UpdateClusterDto> Updates { get; set; } = new();
    }
}
