namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class ScatterPlotResponseDto
    {
        public List<ScatterPlotPointDto> Points { get; set; } = new();

        public List<ScatterPlotCentroidDto> Centroids { get; set; } = new();
    }
}
