namespace DeltaFour.Domain.Entities;

public class ClusterCentroid : BaseEntity
{
    public Guid CompanyId { get; set; }

    public int Cluster { get; set; }

    public double LatePercentage { get; set; }

    public double AverageLateMinutes { get; set; }

    public int MaxLateMinutes { get; set; }

    public int TotalAbsences { get; set; }

    public int TotalWorkedDays { get; set; }

    public DateTime CalculatedAt { get; set; }

    public Company? Company { get; set; }
}
