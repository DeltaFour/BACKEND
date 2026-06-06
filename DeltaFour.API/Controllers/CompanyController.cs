using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeltaFour.API.Controllers;

[Route("api/v1/company")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }
    /// <summary>
    /// Cria uma nova empresa no sistema.
    /// </summary>
    /// <remarks>
    /// O usuário autenticado deve possuir permissão de SUPER_ADMIN.
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await _companyService.Create(request, user.Id);

        return NoContent();
    }

    [HttpGet("settings")]
    [Authorize(Policy = "RH_OR_ADMIN")]
    public async Task<ActionResult<CompanyGetSettingsDto>> Get()
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();
        return Ok(await _companyService.Get(user));
    }

    [HttpPatch("settings")]
    [Authorize(Policy = "RH_OR_ADMIN")]
    public async Task<IActionResult> Update([FromBody] CompanyGetSettingsDto dto)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();
        await _companyService.UpdateSettings(dto, user);
        return NoContent();
    }
        
    /// <summary>
    /// Altera o status (ativo/inativo) de uma empresa.
    /// </summary>
    /// <remarks>
    /// O SUPER_ADMIN pode ativar ou desativar qualquer empresa cadastrada.
    /// </remarks>
    [HttpPost("change-status/{id}")]
    public async Task<IActionResult> Active(Guid id)
    {
        await _companyService.ChangeStatus(id);

        return NoContent();
    }

    /// <summary>
    /// Lista todas as empresas cadastradas no sistema.
    /// </summary>
    /// <remarks>
    /// Retorna as informações de todas as empresas, independentemente do status.
    /// </remarks>
    [HttpGet("list")]
    public async Task<ActionResult<ListCompaniesResponse>> List()
    {
        var companies = await _companyService.List();

        return Ok(companies);
    }
}
