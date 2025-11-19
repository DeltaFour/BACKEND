using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Application.Service.Company;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers.SuperAdmin;

[Route("api/super-admin/company")]
[ApiController]
[Authorize(Policy = nameof(RoleType.SUPER_ADMIN))]
public class CompanyController(StatusService statusService) : ControllerBase
{
    private readonly StatusService _statusService = statusService;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateService createService,
        [FromBody] CreateCompanyRequest request
    )
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await createService.Create(request, user.Id);

        return NoContent();
    }

    [HttpPost("{id}/active")]
    public async Task<IActionResult> Active(Guid id)
    {
        await _statusService.Active(id);

        return NoContent();
    }

    [HttpPost("{id}/desactive")]
    public async Task<IActionResult> Desactive(Guid id)
    {
        await _statusService.Active(id);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ListCompaniesResponse>> List([FromServices] ListService listService)
    {
        var companies = await listService.Get();

        return Ok(companies);
    }
}
