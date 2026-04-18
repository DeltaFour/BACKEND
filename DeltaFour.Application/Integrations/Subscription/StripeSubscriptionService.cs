using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Application.Services;
using Stripe;
using Stripe.Checkout;

namespace DeltaFour.Application.Integrations.Subscription;

public class StripeSubscriptionService : ISubscriptionService
{
    private readonly string _apiKey;

    public StripeSubscriptionService()
    {
        _apiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")!;
        StripeConfiguration.ApiKey = _apiKey;
    }

    public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        try
        {
            var customerOptions = new CustomerCreateOptions
            {
                Email = request.CompanyEmail,
                Name = request.PlanName,
                Metadata = new Dictionary<string, string>
                {
                    { "company_id", request.CompanyId.ToString() }
                }
            };

            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(customerOptions);

            var priceId = Environment.GetEnvironmentVariable("STRIPE_PRICE_ID")!;

            var sessionOptions = new SessionCreateOptions
            {
                Customer = customer.Id,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = Environment.GetEnvironmentVariable("STRIPE_SUCCESS_URL"),
                CancelUrl = Environment.GetEnvironmentVariable("STRIPE_CANCEL_URL"),
                Metadata = new Dictionary<string, string>
                {
                    { "company_id", request.CompanyId.ToString() }
                }
            };

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(sessionOptions);

            return new SubscriptionResult
            {
                Success = true,
                ExternalId = session.Id
            };
        }
        catch (StripeException e)
        {
            return new SubscriptionResult
            {
                Success = false,
                ErrorMessage = e.Message
            };
        }
    }

    public async Task CancelSubscriptionAsync(string externalId)
    {
        var service = new SubscriptionService();
        await service.CancelAsync(externalId);
    }

    public async Task<SubscriptionInfo> GetSubscriptionAsync(string externalId)
    {
        var service = new SubscriptionService();
        var subscription = await service.GetAsync(externalId);

        return new SubscriptionInfo
        {
            ExternalId = subscription.Id,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndedAt
        };
    }
}
