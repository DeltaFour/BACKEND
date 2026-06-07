namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class CentroidDto
    {
        public int Cluster { get; set; }

        public double LatePercentage { get; set; }

        public double AverageLateMinutes { get; set; }

        public double MaxLateMinutes { get; set; }

        public double TotalAbsences { get; set; }

        public double TotalWorkedDays { get; set; }
    }
}
