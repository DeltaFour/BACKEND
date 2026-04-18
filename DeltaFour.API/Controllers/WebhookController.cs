using DeltaFour.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace DeltaFour.API.Controllers;

[Route("api/v1/webhook")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly SubscriptionWebhookService _webhookService;
    private readonly string _webhookSecret;

    public WebhookController(SubscriptionWebhookService webhookService)
    {
        _webhookService = webhookService;
        _webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")!;
    }

    /// <summary>
    /// Webhook para receber eventos do Stripe relacionados a assinaturas.
    /// </summary>
    [HttpPost("subscription")]
    public async Task<IActionResult> HandleSubscriptionWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeSignature = Request.Headers["Stripe-Signature"];

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                _webhookSecret
            );

            await _webhookService.ProcessWebhookEvent(stripeEvent);

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}
