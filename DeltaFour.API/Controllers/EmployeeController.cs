using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.ValueObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    [Route("api/employee")]
    [Authorize]
    [ApiController]
    public class EmployeeController(EmployeeService service) : Controller
    {
        [HttpGet]
        [Authorize(Policy = nameof(RoleType.RH))]
        public async Task<ActionResult<List<EmployeeResponseDto>>> GetAllByCompany()
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.GetAllByCompany(employee.CompanyId));
        }

        [HttpPost]
        [Authorize(Policy = nameof(RoleType.RH))]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto employeeCreateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>(); 
            await service.Create(employeeCreateDto, employee);
            return Ok();
        }

        [HttpPatch]
        [Authorize(Policy = nameof(RoleType.RH))]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(employeeUpdateDto, employee);
            return Ok();
        }

        [HttpDelete("{employeeId}")]
        [Authorize(Policy = nameof(RoleType.RH))]
        public async Task<IActionResult> Delete(Guid employeeId)
        {
            await service.Delete(employeeId);
            return Ok();
        }

        [HttpGet("allowed-punch")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<ActionResult<Boolean>> CheckIfCanPunchIn([FromBody] CanPunchDto dto)
        {   
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.CanPunchIn(dto, user)); 
        }

        [HttpPost("punch-in")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<IActionResult> PunchIn([FromBody] PunchDto punchDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            PunchInResponse response = await service.PunchIn(punchDto, user);
            if (response == PunchInResponse.SCC)
            {
                return Ok(response.Message());
            }

            return Unauthorized(response.Message());
        }

        [HttpPost("punch-for-user")]
        [Authorize(Policy = nameof(RoleType.RH))]
        public async Task<IActionResult> PunchForUser([FromBody] PunchForUserDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await  service.PunchForUser(dto, user);
            return Ok();
        }
    }
}