using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;

namespace DeltaFour.Application.Services;

public interface ISubscriptionService
{
    Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request);
    Task CancelSubscriptionAsync(string externalId);
    Task<SubscriptionInfo> GetSubscriptionAsync(string externalId);
    Task<string?> CreateBillingPortalSessionAsync(string customerId, string returnUrl);
    Task<SubscriptionResult> ReactivateSubscriptionAsync(string customerId, Guid companyId);
    Task<string?> CreatePaymentMethodUpdateSessionAsync(string customerId, string returnUrl);
}
