using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Application.Service.Company;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers.SuperAdmin;

[Route("api/v1/admin-control/company")]
[ApiController]
[Authorize(Policy = nameof(RoleType.SUPER_ADMIN))]
public class CompanyController(StatusService statusService) : ControllerBase
{

    ///<sumary>
    ///Create company
    ///</sumary>
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromServices] CreateService createService,
        [FromBody] CreateCompanyRequest request
    )
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await createService.Create(request, user.Id);

        return NoContent();
    }

    ///<sumary>
    ///Activate or disable company
    ///</sumary>
    [HttpPost("change-status/{id}")]
    public async Task<IActionResult> Active(Guid id)
    {
        await statusService.ChangeStatus(id);

        return NoContent();
    }
    
    ///<sumary>
    ///List all companies registered
    ///</sumary>
    [HttpGet("list")]
    public async Task<ActionResult<ListCompaniesResponse>> List([FromServices] ListService listService)
    {
        var companies = await listService.Get();

        return Ok(companies);
    }
}
