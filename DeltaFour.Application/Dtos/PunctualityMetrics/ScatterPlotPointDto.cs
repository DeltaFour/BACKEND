namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class ScatterPlotPointDto
    {
        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        public double LatePercentage { get; set; }

        public double AverageLateMinutes { get; set; }

        public int? Cluster { get; set; }
    }
}
