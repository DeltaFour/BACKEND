namespace DeltaFour.Application.Dtos.PunctualityMetrics
{
    public class PunctualityMetricExportDto
    {
        public Guid UserId { get; set; }

        public Guid CompanyId { get; set; }

        public double LatePercentage { get; set; }

        public double AverageLateMinutes { get; set; }

        public int MaxLateMinutes { get; set; }

        public int TotalAbsences { get; set; }

        public int TotalWorkedDays { get; set; }
    }
}
