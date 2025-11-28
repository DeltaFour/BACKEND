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
        /// <summary>
        /// Lista todos os usuários vinculados à empresa do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpGet("list")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllByCompany()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.GetAllByCompany(user.CompanyId));
        }

        /// <summary>
        /// Cria um novo usuário dentro da empresa do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Disponível apenas para ADMIN ou RH.
        /// </remarks>
        [HttpPost("create")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userCreateDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>(); 
            await service.Create(userCreateDto, user);
            return Ok();
        }

        /// <summary>
        /// Atualiza as informações de um usuário pertencente à empresa.
        /// </summary>
        /// <remarks>
        /// Apenas ADMIN ou RH podem realizar a atualização.
        /// </remarks>
        [HttpPatch("update")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(userUpdateDto, user);
            return Ok();
        }

        /// <summary>
        /// Altera o status de um usuário (ativo/inativo).
        /// </summary>
        /// <remarks>
        /// Apenas ADMIN ou RH podem desativar ou reativar um usuário.
        /// </remarks>
        [HttpDelete("change-status/{userId}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> ChangeStatus(Guid userId)
        {
            await service.Delete(userId);
            return Ok();
        }

        /// <summary>
        /// Verifica se o usuário está autorizado a registrar um novo ponto.
        /// </summary>
        /// <remarks>
        /// Disponível para ADMIN, RH ou EMPLOYEE.
        /// </remarks>
        [HttpPost("allowed-punch")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<ActionResult<Boolean>> CheckIfCanPunchIn([FromBody] CanPunchDto dto)
        {   
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.CanPunchIn(dto, user)); 
        }

        /// <summary>
        /// Registra o ponto de entrada ou saída do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Realiza validações e retorna mensagens de erro caso o registro não seja permitido.
        /// </remarks>
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

        /// <summary>
        /// Atualiza e retorna informações do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Utilizado geralmente após login ou quando é necessário atualizar dados locais.
        /// </remarks>
        [HttpGet("refresh-information")]
        [Authorize(Policy = "RH_OR_EMPLOYEE")]
        public async Task<UserInfoLoginDto> RefreshInformation()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return await service.RefreshUserInformation(user);
        }

        /// <summary>
        /// Permite que um usuário autorizado registre o ponto para outro usuário específico.
        /// </summary>
        /// <remarks>
        /// Apenas ADMIN ou RH podem utilizar este endpoint.
        /// </remarks>
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