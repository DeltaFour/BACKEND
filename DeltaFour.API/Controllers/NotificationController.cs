using DeltaFour.Application.Dtos.Notifications;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    [Route("api/v1/notifications")]
    [Authorize]
    [ApiController]
    public class NotificationController(NotificationService service) : Controller
    {
        /// <summary>
        /// Lista as notificações recentes (últimas 24h) da empresa do usuário autenticado.
        /// Usada para repovoar a dashboard ao recarregar a página.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<List<NotificationDto>>> GetRecent()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            return Ok(await service.GetRecentForCompany(user.CompanyId));
        }

        /// <summary>
        /// Marca todas as notificações da empresa como lidas.
        /// </summary>
        [HttpPatch("read-all")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> MarkAllRead()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.MarkAllAsRead(user.CompanyId);
            return NoContent();
        }
    }
}
