using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
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

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto employeeCreateDto, List<IFormFile> files) 
        {
            var employee = HttpContext.GetUserAuthenticated<Employee>();
            await service.Create(employeeCreateDto, files, employee);
            return Ok();
        }

        public async Task<IActionResult> Update([FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = HttpContext.GetUserAuthenticated<Employee>();
            await service.Update(employeeUpdateDto, employee.CompanyId);
            return Ok();
        }
    }
}
