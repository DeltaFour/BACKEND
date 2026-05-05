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
                Name = request.CustomerName,
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
                CheckoutSessionId = session.Id,
                CheckoutUrl = session.Url,
                CustomerId = customer.Id
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
        var subscriptionService = new SubscriptionService();

        string? subscriptionId = externalId;

        // If a Checkout Session id (cs_...) was passed, resolve it to the created subscription id
        if (!string.IsNullOrEmpty(externalId) && externalId.StartsWith("cs_"))
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(externalId);
            subscriptionId = session?.SubscriptionId;
        }

        // If a customer id (cus_...) was passed, try to find an active subscription for that customer
        if (!string.IsNullOrEmpty(externalId) && externalId.StartsWith("cus_") && string.IsNullOrEmpty(subscriptionId))
        {
            var listOptions = new SubscriptionListOptions
            {
                Customer = externalId,
                Limit = 1
            };

            var list = await subscriptionService.ListAsync(listOptions);
            subscriptionId = list.Data.FirstOrDefault()?.Id;
        }

        if (string.IsNullOrEmpty(subscriptionId))
            throw new Exception("Subscription id could not be resolved for cancellation");

        await subscriptionService.CancelAsync(subscriptionId);
    }

    public async Task<SubscriptionInfo> GetSubscriptionAsync(string externalId)
    {
        var subscriptionService = new SubscriptionService();

        string? subscriptionId = externalId;

        if (!string.IsNullOrEmpty(externalId) && externalId.StartsWith("cs_"))
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(externalId);
            subscriptionId = session?.SubscriptionId;
        }

        if (!string.IsNullOrEmpty(externalId) && externalId.StartsWith("cus_") && string.IsNullOrEmpty(subscriptionId))
        {
            var listOptions = new SubscriptionListOptions
            {
                Customer = externalId,
                Limit = 1
            };

            var list = await subscriptionService.ListAsync(listOptions);
            subscriptionId = list.Data.FirstOrDefault()?.Id;
        }

        if (string.IsNullOrEmpty(subscriptionId))
            throw new Exception("Subscription id could not be resolved");

        var subscription = await subscriptionService.GetAsync(subscriptionId);

        return new SubscriptionInfo
        {
            ExternalId = subscription.Id,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndedAt
        };
    }

    public async Task<string?> CreateBillingPortalSessionAsync(string subscriptionId, string returnUrl)
    {
        if (string.IsNullOrEmpty(subscriptionId))
            return null;

        var subscriptionService = new SubscriptionService();

        string? customerId = null;

        if (subscriptionId.StartsWith("cs_"))
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(subscriptionId);
            customerId = session?.CustomerId;
        }
        else if (subscriptionId.StartsWith("sub_"))
        {
            var subscription = await subscriptionService.GetAsync(subscriptionId);
            customerId = subscription?.CustomerId;
        }
        else if (subscriptionId.StartsWith("cus_"))
        {
            customerId = subscriptionId;
        }

        if (string.IsNullOrEmpty(customerId))
            throw new Exception("Customer id could not be resolved for billing portal");

        var portalService = new Stripe.BillingPortal.SessionService();
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl
        };

        var portalSession = await portalService.CreateAsync(options);

        return portalSession.Url;
    }

    public async Task<SubscriptionResult> ReactivateSubscriptionAsync(string customerId, Guid companyId)
    {
        try
        {
            var priceId = Environment.GetEnvironmentVariable("STRIPE_PRICE_ID")!;

            var sessionOptions = new SessionCreateOptions
            {
                Customer = customerId,
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
                    { "company_id", companyId.ToString() }
                }
            };

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(sessionOptions);

            return new SubscriptionResult
            {
                Success = true,
                CheckoutSessionId = session.Id,
                CheckoutUrl = session.Url,
                CustomerId = customerId
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

    public async Task<string?> CreatePaymentMethodUpdateSessionAsync(string customerId, string returnUrl)
    {
        if (string.IsNullOrEmpty(customerId))
            return null;

        var portalService = new Stripe.BillingPortal.SessionService();
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl,
            FlowData = new Stripe.BillingPortal.SessionFlowDataOptions
            {
                Type = "payment_method_update"
            }
        };

        var portalSession = await portalService.CreateAsync(options);

        return portalSession.Url;
    }
}
