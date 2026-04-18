namespace DeltaFour.Domain.Entities;

public class SubscriptionEvent : BaseEntity
{
    public Guid SubscriptionId { get; set; }

    public string EventType { get; set; }

    public string Payload { get; set; }

    public Subscription Subscription { get; set; }
}
