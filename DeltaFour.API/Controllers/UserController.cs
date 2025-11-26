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
    [Route("api/v1/user")]
    [Authorize]
    [ApiController]
    public class UserController(EmployeeService service) : Controller
    {
        [HttpGet("list")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<List<EmployeeResponseDto>>> GetAllByCompany()
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.GetAllByCompany(employee.CompanyId));
        }

        [HttpPost("create")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto employeeCreateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>(); 
            await service.Create(employeeCreateDto, employee);
            return Ok();
        }

        [HttpPatch("update")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(employeeUpdateDto, employee);
            return Ok();
        }

        [HttpDelete("change-status/{employeeId}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> ChangeStatus(Guid employeeId)
        {
            await service.Delete(employeeId);
            return Ok();
        }

        [HttpPost("allowed-punch")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<ActionResult<Boolean>> CheckIfCanPunchIn([FromBody] CanPunchDto dto)
        {   
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.CanPunchIn(dto, user)); 
        }

        [HttpPost("register-point")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<IActionResult> PunchIn([FromBody] PunchDto punchDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            PunchInResponse response = await service.PunchIn(punchDto, user);
            if (response == PunchInResponse.SCC)
            {
                return Ok(response.Message());
            }

            return BadRequest(response.Message());
        }

        [HttpGet("refresh-information")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<UserInfoForRefresh> RefreshInformation()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return await service.RefreshUserInformation(user);
        }

        [HttpPost("punch-for-user")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> PunchForUser([FromBody] PunchForUserDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await  service.PunchForUser(dto, user);
            return Ok();
        }
    }
}