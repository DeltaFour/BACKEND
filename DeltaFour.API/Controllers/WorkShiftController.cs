using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    public class WorkShiftController(WorkShiftService service) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WorkShiftDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<Employee>();
            await service.Create(dto, user);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] WorkShiftUpdateDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<Employee>();
            await service.Update(dto, user);
            return Ok();
        }
    }
}
