namespace DeltaFour.Domain.Entities;

public class UserPunctualityMetric : BaseEntity
{
    public Guid UserId { get; set; }

    public int TotalAttendances { get; set; }

    public int TotalLateAttendances { get; set; }

    public double LatePercentage { get; set; }

    public double AverageLateMinutes { get; set; }

    public int MaxLateMinutes { get; set; }

    public int TotalAbsences { get; set; }

    public int TotalWorkedDays { get; set; }

    public int? Cluster { get; set; }

    public DateTime LastCalculatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }
}
