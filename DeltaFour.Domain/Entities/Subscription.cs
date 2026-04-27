namespace DeltaFour.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid CompanyId { get; set; }

    public string PlanName { get; set; }

    public string Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ExternalId { get; set; }

    public string? CustomerId { get; set; }

    public Company Company { get; set; }

    public List<SubscriptionEvent>? SubscriptionEvents { get; set; }
}
