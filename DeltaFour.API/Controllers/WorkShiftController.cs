using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    [Route("api/v1/workshift")]
    [ApiController]
    public class WorkShiftController(WorkShiftService service) : Controller
    {
        ///<sumary>
        ///List all WorkShifts from company
        ///</sumary>
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<List<WorkShiftResponseDto>?>> Get()
        {
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            var workshifts = await service.Get(user.CompanyId);

            return Ok(workshifts ?? new List<WorkShiftResponseDto>());
        }
        
        ///<sumary>
        ///Create WorkShift
        ///</sumary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] WorkShiftDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Create(dto, user);
            return Ok();
        }

        ///<sumary>
        ///Update Workshift
        ///</sumary>
        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] WorkShiftUpdateDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(dto, user);
            return Ok();
        }

        ///<sumary>
        ///Change the status from WorkShift
        ///</sumary>
        [HttpDelete("change-status/{workShiftId}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid workShiftId)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Delete(workShiftId, user.CompanyId);
            return Ok();
        }
    }
}
