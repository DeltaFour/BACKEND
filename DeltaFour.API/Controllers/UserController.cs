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
    public class UserController(UserService service) : Controller
    {
        ///<sumary>
        ///List all user from company
        ///</sumary>
        [HttpGet("list")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllByCompany()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.GetAllByCompany(user.CompanyId));
        }

        ///<sumary>
        ///Create user for company
        ///</sumary>
        [HttpPost("create")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userCreateDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>(); 
            await service.Create(userCreateDto, user);
            return Ok();
        }

        ///<sumary>
        ///Update user of company
        ///</sumary>
        [HttpPatch("update")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(userUpdateDto, user);
            return Ok();
        }

        ///<sumary>
        ///Change the status from a specific user
        ///</sumary>
        [HttpDelete("change-status/{userId}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> ChangeStatus(Guid userId)
        {
            await service.Delete(userId);
            return Ok();
        }

        ///<sumary>
        ///Check if user can punch
        ///</sumary>
        [HttpPost("allowed-punch")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<ActionResult<Boolean>> CheckIfCanPunchIn([FromBody] CanPunchDto dto)
        {   
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.CanPunchIn(dto, user)); 
        }

        ///<sumary>
        ///Punch for user
        ///</sumary>
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

        ///<sumary>
        ///Refresh information of user logged
        ///</sumary>
        [HttpGet("refresh-information")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<UserInfoLoginDto> RefreshInformation()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return await service.RefreshUserInformation(user);
        }

        ///<sumary>
        ///Other user can punch for specific user
        ///</sumary>
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