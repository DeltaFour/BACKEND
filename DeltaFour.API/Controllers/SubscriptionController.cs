using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers;

[Route("api/v1/subscription")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly CompanyRegistrationService _companyRegistrationService;

    public SubscriptionController(CompanyRegistrationService companyRegistrationService)
    {
        _companyRegistrationService = companyRegistrationService;
    }

    /// <summary>
    /// Registra uma nova empresa com criação automática de assinatura.
    /// </summary>
    /// <remarks>
    /// Endpoint público que permite o cadastro de empresas sem necessidade de autenticação.
    /// Cria a empresa, usuário admin e inicia o processo de assinatura.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<SubscriptionResult>> Register([FromBody] RegisterCompanyRequest request)
    {
        var result = await _companyRegistrationService.RegisterCompanyWithSubscription(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Retorna os dados da assinatura da empresa do usuário autenticado.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SubscriptionInfo>> GetSubscription()
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        var subscription = await _companyRegistrationService.GetCompanySubscription(user.CompanyId);

        if (subscription == null)
            return NotFound(new { message = "Subscription not found" });

        return Ok(subscription);
    }

    /// <summary>
    /// Cancela a assinatura da empresa do usuário autenticado.
    /// </summary>
    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> CancelSubscription()
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await _companyRegistrationService.CancelCompanySubscription(user.CompanyId);

        return NoContent();
    }

    /// <summary>
    /// Reativa uma assinatura cancelada da empresa do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Cria uma nova sessão de checkout para reativar a assinatura cancelada.
    /// Retorna a URL de checkout para o usuário completar o pagamento.
    /// </remarks>
    [HttpPost("reactivate")]
    [Authorize]
    public async Task<ActionResult<SubscriptionResult>> ReactivateSubscription()
    {
        var user = HttpContext.GetUserAuthenticated<Domain.Entities.UserContext>();

        var result = await _companyRegistrationService.ReactivateCompanySubscription(user.CompanyId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Retorna a URL do portal de cobrança do Stripe para gerenciar a assinatura.
    /// </summary>
    /// <remarks>
    /// Permite ao usuário visualizar faturas, histórico de pagamentos e gerenciar a assinatura.
    /// </remarks>
    [HttpGet("billing-portal")]
    [Authorize]
    public async Task<ActionResult<BillingPortalResponse>> GetBillingPortal([FromQuery] string? returnUrl)
    {
        var user = HttpContext.GetUserAuthenticated<Domain.Entities.UserContext>();

        var defaultReturnUrl = returnUrl ?? Environment.GetEnvironmentVariable("STRIPE_SUCCESS_URL") ?? "/";

        var url = await _companyRegistrationService.GetBillingPortalUrl(user.CompanyId, defaultReturnUrl);

        if (string.IsNullOrEmpty(url))
            return NotFound(new { message = "Could not create billing portal session" });

        return Ok(new BillingPortalResponse { Url = url });
    }

    /// <summary>
    /// Retorna a URL para atualizar a forma de pagamento no Stripe.
    /// </summary>
    /// <remarks>
    /// Redireciona o usuário para o portal do Stripe onde pode atualizar cartão de crédito ou outra forma de pagamento.
    /// </remarks>
    [HttpGet("update-payment-method")]
    [Authorize]
    public async Task<ActionResult<BillingPortalResponse>> GetUpdatePaymentMethodUrl([FromQuery] string? returnUrl)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        var defaultReturnUrl = returnUrl ?? Environment.GetEnvironmentVariable("STRIPE_SUCCESS_URL") ?? "/";

        var url = await _companyRegistrationService.GetPaymentMethodUpdateUrl(user.CompanyId, defaultReturnUrl);

        if (string.IsNullOrEmpty(url))
            return NotFound(new { message = "Could not create payment method update session" });

        return Ok(new BillingPortalResponse { Url = url });
    }
}
