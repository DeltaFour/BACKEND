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
        /// <summary>
        /// Lista todos os turnos de trabalho da empresa do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Retorna todos os WorkShifts ativos e inativos.
        /// </remarks>
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<List<WorkShiftResponseDto>?>> Get()
        {
            var user =  HttpContext.GetUserAuthenticated<UserContext>();
            var workshifts = await service.Get(user.CompanyId);

            return Ok(workshifts ?? new List<WorkShiftResponseDto>());
        }
        
        /// <summary>
        /// Cria um novo turno de trabalho para a empresa.
        /// </summary>
        /// <remarks>
        /// Apenas usuários autenticados podem criar turnos.
        /// </remarks>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] WorkShiftDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Create(dto, user);
            return Ok();
        }

        /// <summary>
        /// Atualiza as informações de um turno de trabalho existente.
        /// </summary>
        /// <remarks>
        /// Permite alterar horários, tolerâncias e demais dados.
        /// </remarks>
        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] WorkShiftUpdateDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.Update(dto, user);
            return Ok();
        }

        /// <summary>
        /// Altera o status de um turno de trabalho (ativo/inativo).
        /// </summary>
        /// <remarks>
        /// O turno deixa de ser utilizado imediatamente ao ser desativado.
        /// </remarks>
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
