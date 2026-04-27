using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using Stripe;
using System.Text.Json;

namespace DeltaFour.Application.Services;

public class SubscriptionWebhookService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionWebhookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessWebhookEvent(Event stripeEvent)
    {
        var eventPayload = JsonSerializer.Serialize(stripeEvent.Data.Object);

        switch (stripeEvent.Type)
        {
            case "checkout.session.completed":
                await HandleCheckoutSessionCompleted(stripeEvent, eventPayload);
                break;

            case "customer.subscription.updated":
                await HandleSubscriptionUpdated(stripeEvent, eventPayload);
                break;

            case "customer.subscription.deleted":
                await HandleSubscriptionDeleted(stripeEvent, eventPayload);
                break;

            case "invoice.payment_failed":
                await HandlePaymentFailed(stripeEvent, eventPayload);
                break;

            case "invoice.payment_succeeded":
                await HandlePaymentSucceeded(stripeEvent, eventPayload);
                break;
            default:
                break;
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent, string payload)
    {
        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

        if (session?.Metadata?.TryGetValue("company_id", out var companyIdStr) == true)
        {
            var companyId = Guid.Parse(companyIdStr);
            var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

            if (subscription != null && session.SubscriptionId != null)
            {
                subscription.Status = SubscriptionStatus.ACTIVE.ToString();
                subscription.ExternalId = session.SubscriptionId;
                subscription.CustomerId = session.CustomerId;
                subscription.EndDate = null;

                var subscriptionEvent = new Domain.Entities.SubscriptionEvent
                {
                    SubscriptionId = subscription.Id,
                    EventType = stripeEvent.Type,
                    Payload = payload
                };

                _unitOfWork.SubscriptionEventRepository.Create(subscriptionEvent);
                await _unitOfWork.Save();
            }
        }
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent, string payload)
    {
        var stripeSubscription = stripeEvent.Data.Object as Subscription;

        if (stripeSubscription != null)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.Find(
                s => s.ExternalId == stripeSubscription.Id);

            if (subscription != null)
            {
                subscription.Status = MapStripeStatus(stripeSubscription.Status);

                var subscriptionEvent = new Domain.Entities.SubscriptionEvent
                {
                    SubscriptionId = subscription.Id,
                    EventType = stripeEvent.Type,
                    Payload = payload
                };

                _unitOfWork.SubscriptionEventRepository.Create(subscriptionEvent);
                await _unitOfWork.Save();
            }
        }
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent, string payload)
    {
        var stripeSubscription = stripeEvent.Data.Object as Subscription;

        if (stripeSubscription != null)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.Find(
                s => s.ExternalId == stripeSubscription.Id);

            if (subscription != null)
            {
                subscription.Status = SubscriptionStatus.CANCELED.ToString();
                subscription.EndDate = DateTime.UtcNow;

                var subscriptionEvent = new Domain.Entities.SubscriptionEvent
                {
                    SubscriptionId = subscription.Id,
                    EventType = stripeEvent.Type,
                    Payload = payload
                };

                _unitOfWork.SubscriptionEventRepository.Create(subscriptionEvent);
                await _unitOfWork.Save();
            }
        }
    }

    private async Task HandlePaymentFailed(Event stripeEvent, string payload)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        var invoiceSubscriptionId = GetSubscriptionIdFromInvoice(invoice);

        if (invoice?.Id != null && invoiceSubscriptionId != null)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.Find(
                s => s.ExternalId == invoiceSubscriptionId);

            if (subscription != null)
            {
                subscription.Status = SubscriptionStatus.PAST_DUE.ToString();

                var subscriptionEvent = new Domain.Entities.SubscriptionEvent
                {
                    SubscriptionId = subscription.Id,
                    EventType = stripeEvent.Type,
                    Payload = payload
                };

                _unitOfWork.SubscriptionEventRepository.Create(subscriptionEvent);
                await _unitOfWork.Save();
            }
        }
    }

    private async Task HandlePaymentSucceeded(Event stripeEvent, string payload)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        var invoiceSubscriptionId = GetSubscriptionIdFromInvoice(invoice);

        if (invoiceSubscriptionId != null)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.Find(
                s => s.ExternalId == invoiceSubscriptionId);

            if (subscription != null)
            {
                if (subscription.Status == SubscriptionStatus.PAST_DUE.ToString())
                {
                    subscription.Status = SubscriptionStatus.ACTIVE.ToString();
                }

                var subscriptionEvent = new Domain.Entities.SubscriptionEvent
                {
                    SubscriptionId = subscription.Id,
                    EventType = stripeEvent.Type,
                    Payload = payload
                };

                _unitOfWork.SubscriptionEventRepository.Create(subscriptionEvent);
                await _unitOfWork.Save();
            }
        }
    }

    private static string? GetSubscriptionIdFromInvoice(Invoice? invoice)
    {
        if (invoice?.Parent == null)
            return null;

        if (invoice.Parent.Type == "subscription" && invoice.Parent.SubscriptionDetails?.Subscription != null)
            return invoice.Parent.SubscriptionDetails.Subscription.Id;

        return null;
    }

    private string MapStripeStatus(string stripeStatus)
    {
        return stripeStatus.ToLower() switch
        {
            "active" => SubscriptionStatus.ACTIVE.ToString(),
            "past_due" => SubscriptionStatus.PAST_DUE.ToString(),
            "canceled" => SubscriptionStatus.CANCELED.ToString(),
            "incomplete" => SubscriptionStatus.PENDING.ToString(),
            _ => SubscriptionStatus.PENDING.ToString()
        };
    }
}
