using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    [Route("api/employee")]
    [Authorize]
    public class EmployeeController(EmployeeService service) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<EmployeeResponseDto>>> GetAllByCompany()
        {
            var employee = HttpContext.GetUserAuthenticated<Employee>();
            return Ok(await service.GetAllByCompany(employee.CompanyId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto employeeCreateDto, List<IFormFile> files)
        {
            var employee = HttpContext.GetUserAuthenticated<Employee>();
            await service.Create(employeeCreateDto, files, employee);
            return Ok();
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<Employee>();
            await service.Update(employeeUpdateDto, employee);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(Guid employeeId)
        {
            await service.Delete(employeeId);
            return Ok();
        }

        [HttpPost("punch-in")]
        [Authorize]
        public async Task<IActionResult> PunchIn([FromBody] PunchDto punchDto, [FromBody] IFormFile file)
        {
            var user = HttpContext.GetUserAuthenticated<Employee>();

            if (await service.PunchIn(punchDto, user))
            {
                return Ok();
            }
            
            return Forbid();
        }
    }
}
