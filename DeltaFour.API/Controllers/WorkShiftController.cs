using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    public class WorkShiftController(WorkShiftService service) : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody] WorkShiftDto dto)
        {
            service.
        }
    }
}
