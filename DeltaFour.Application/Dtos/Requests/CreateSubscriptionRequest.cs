namespace DeltaFour.Application.Dtos.Requests;

public class CreateSubscriptionRequest
{
    public Guid CompanyId { get; set; }
    public string PlanName { get; set; }
    public string CompanyEmail { get; set; }
    public string CustomerName { get; set; }
}
