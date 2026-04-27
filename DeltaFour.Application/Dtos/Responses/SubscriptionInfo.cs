namespace DeltaFour.Application.Dtos.Responses;

public class SubscriptionInfo
{
    public Guid Id { get; set; }
    public string PlanName { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ExternalId { get; set; }
    public string? CustomerId { get; set; }
}
