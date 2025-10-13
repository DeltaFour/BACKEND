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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> create([FromBody] EmployeeCreateDto employeeCreateDto, List<IFormFile> files)
        {
            var employee = HttpContext.GetObject<Employee>();
            await service.create(employeeCreateDto, files, employee);
            return Ok();
        }
    }
}
