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
}
