namespace DeltaFour.Domain.Entities;

public class ClusterCentroid : BaseEntity
{
    public Guid CompanyId { get; set; }

    public int Cluster { get; set; }

    public double LatePercentage { get; set; }

    public double AverageLateMinutes { get; set; }

    public double MaxLateMinutes { get; set; }

    public double TotalAbsences { get; set; }

    public double TotalWorkedDays { get; set; }

    public DateTime CalculatedAt { get; set; }

    public Company? Company { get; set; }
}
