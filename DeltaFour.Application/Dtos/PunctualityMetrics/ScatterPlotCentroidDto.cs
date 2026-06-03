namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class ScatterPlotCentroidDto
    {
        public int Cluster { get; set; }

        public double LatePercentage { get; set; }

        public double AverageLateMinutes { get; set; }
    }
}
