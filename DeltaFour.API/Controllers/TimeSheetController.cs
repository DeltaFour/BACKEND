using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    /// <summary>
    /// Controller para geração de relatórios de folha de ponto
    /// </summary>
    [Route("api/v1/timesheet")]
    [Authorize]
    [ApiController]
    public class TimeSheetController : Controller
    {
        private readonly ITimeSheetPdfService _timeSheetService;

        public TimeSheetController(ITimeSheetPdfService timeSheetService)
        {
            _timeSheetService = timeSheetService;
        }

        /// <summary>
        /// Gera a folha de ponto em PDF para um funcionário específico
        /// </summary>
        /// <param name="userId">ID do funcionário</param>
        /// <param name="month">Mês da competência (1-12)</param>
        /// <param name="year">Ano da competência</param>
        /// <returns>Arquivo PDF da folha de ponto</returns>
        /// <remarks>
        /// Disponível para ADMIN, RH ou o próprio funcionário.
        /// O PDF inclui:
        /// - Dados da empresa e funcionário
        /// - Tabela com todos os dias do mês
        /// - Horários de entrada e saída
        /// - Horas trabalhadas, esperadas e saldo
        /// - Totalizadores do período
        /// - Área para assinaturas
        /// </remarks>
        [HttpGet("pdf/{userId:guid}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> GeneratePdf(
            [FromRoute] Guid userId,
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var request = new TimeSheetRequestDto
            {
                UserId = userId,
                Month = month,
                Year = year
            };

            var pdfBytes = await _timeSheetService.GenerateTimeSheetAsync(request);

            var fileName = $"FolhaPonto_{year}{month:D2}_{userId}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Gera a folha de ponto em PDF para o usuário autenticado
        /// </summary>
        /// <param name="month">Mês da competência (1-12)</param>
        /// <param name="year">Ano da competência</param>
        /// <returns>Arquivo PDF da folha de ponto</returns>
        [HttpGet("pdf/me")]
        public async Task<IActionResult> GenerateMyPdf(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();

            var request = new TimeSheetRequestDto
            {
                UserId = user.Id,
                Month = month,
                Year = year
            };

            var pdfBytes = await _timeSheetService.GenerateTimeSheetAsync(request);

            var fileName = $"FolhaPonto_{year}{month:D2}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Obtém os dados da folha de ponto sem gerar o PDF
        /// </summary>
        /// <param name="userId">ID do funcionário</param>
        /// <param name="month">Mês da competência (1-12)</param>
        /// <param name="year">Ano da competência</param>
        /// <returns>Dados estruturados da folha de ponto</returns>
        /// <remarks>
        /// Útil para visualização em tela antes de gerar o PDF.
        /// Disponível para ADMIN, RH ou o próprio funcionário.
        /// </remarks>
        [HttpGet("data/{userId:guid}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<TimeSheetDataDto>> GetTimeSheetData(
            [FromRoute] Guid userId,
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var request = new TimeSheetRequestDto
            {
                UserId = userId,
                Month = month,
                Year = year
            };

            var data = await _timeSheetService.GetTimeSheetDataAsync(request);

            return Ok(data);
        }

        /// <summary>
        /// Obtém os dados da folha de ponto do usuário autenticado
        /// </summary>
        /// <param name="month">Mês da competência (1-12)</param>
        /// <param name="year">Ano da competência</param>
        /// <returns>Dados estruturados da folha de ponto</returns>
        [HttpGet("data/me")]
        public async Task<ActionResult<TimeSheetDataDto>> GetMyTimeSheetData(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();

            var request = new TimeSheetRequestDto
            {
                UserId = user.Id,
                Month = month,
                Year = year
            };

            var data = await _timeSheetService.GetTimeSheetDataAsync(request);

            return Ok(data);
        }
    }
}
