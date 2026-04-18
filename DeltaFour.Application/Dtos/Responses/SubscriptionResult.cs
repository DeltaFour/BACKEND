namespace DeltaFour.Application.Dtos.Responses;

public class SubscriptionResult
{
    public bool Success { get; set; }
    public string? ExternalId { get; set; }
    public string? ErrorMessage { get; set; }
}
