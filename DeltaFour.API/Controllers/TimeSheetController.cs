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
        /// Lista todas as folhas de ponto existentes, com filtros opcionais por usuário, mês e ano.
        /// Este endpoint não exige permissão específica por role.
        /// </summary>
        [HttpGet("list")]
        public async Task<ActionResult<List<TimeSheetListItemDto>>> List(
            [FromQuery] Guid? userId,
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var results = await _timeSheetService.ListTimeSheetsAsync(userId, month, year);
            return Ok(results);
        }

        /// <summary>
        /// Gera a folha de ponto em PDF para um funcionário específico
        /// </summary>
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

        /// <summary>
        /// Assina a folha de ponto pelo funcionário
        /// </summary>
        /// <param name="timeSheetId">ID da folha de ponto</param>
        [HttpPost("{timeSheetId:guid}/sign/employee")]
        public async Task<IActionResult> SignByEmployee([FromRoute] Guid timeSheetId)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();

            await _timeSheetService.SignByEmployeeAsync(timeSheetId, user.Id);

            return Ok(new { message = "Folha de ponto assinada com sucesso pelo funcionário." });
        }

        /// <summary>
        /// Assina a folha de ponto pelo RH
        /// </summary>
        /// <param name="timeSheetId">ID da folha de ponto</param>
        [HttpPost("{timeSheetId:guid}/sign/hr")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> SignByHR([FromRoute] Guid timeSheetId)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();

            await _timeSheetService.SignByHRAsync(timeSheetId, user.Id, user.Name ?? "RH");

            return Ok(new { message = "Folha de ponto assinada com sucesso pelo RH." });
        }

        /// <summary>
        /// Obtém o status de assinatura da folha de ponto
        /// </summary>
        [HttpGet("status/{userId:guid}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> GetSignatureStatus(
            [FromRoute] Guid userId,
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var timeSheet = await _timeSheetService.GetTimeSheetRecordAsync(userId, month, year);

            if (timeSheet == null)
            {
                return Ok(new
                {
                    exists = false,
                    signedByEmployee = false,
                    signedByHR = false
                });
            }

            return Ok(new
            {
                exists = true,
                timeSheetId = timeSheet.Id,
                signedByEmployee = timeSheet.SignedByEmployee,
                employeeSignedAt = timeSheet.EmployeeSignedAt,
                signedByHR = timeSheet.SignedByHR,
                hrSignedAt = timeSheet.HRSignedAt,
                hrSignerName = timeSheet.SignedByHRUserName
            });
        }

        /// <summary>
        /// Obtém o status de assinatura da folha de ponto do usuário autenticado
        /// </summary>
        [HttpGet("status/me")]
        public async Task<IActionResult> GetMySignatureStatus(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            var timeSheet = await _timeSheetService.GetTimeSheetRecordAsync(user.Id, month, year);

            if (timeSheet == null)
            {
                return Ok(new
                {
                    exists = false,
                    signedByEmployee = false,
                    signedByHR = false
                });
            }

            return Ok(new
            {
                exists = true,
                timeSheetId = timeSheet.Id,
                signedByEmployee = timeSheet.SignedByEmployee,
                employeeSignedAt = timeSheet.EmployeeSignedAt,
                signedByHR = timeSheet.SignedByHR,
                hrSignedAt = timeSheet.HRSignedAt,
                hrSignerName = timeSheet.SignedByHRUserName
            });
        }
    }
}
