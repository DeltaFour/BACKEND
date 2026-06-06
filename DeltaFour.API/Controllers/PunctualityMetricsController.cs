using DeltaFour.API.Filters;
using DeltaFour.Application.Dtos.PunctualityMetrics;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de métricas de pontualidade para classificação K-Means
    /// </summary>
    [Route("api/v1/punctuality-metrics")]
    [Authorize]
    [ApiController]
    public class PunctualityMetricsController : Controller
    {
        private readonly PunctualityMetricsService _service;

        public PunctualityMetricsController(PunctualityMetricsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todas as métricas de pontualidade dos usuários da empresa.
        /// </summary>
        /// <remarks>
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpGet("list")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<List<PunctualityMetricResponseDto>>> GetAllByCompany()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            var metrics = await _service.GetMetricsByCompany(user.CompanyId);
            return Ok(metrics);
        }

        /// <summary>
        /// Retorna a métrica de pontualidade de um usuário específico.
        /// </summary>
        /// <remarks>
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpGet("{userId:guid}")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<PunctualityMetricResponseDto>> GetByUserId(Guid userId)
        {
            var metric = await _service.GetMetricByUserId(userId);

            if (metric == null)
                return NotFound("Métricas não encontradas para o usuário.");

            return Ok(metric);
        }

        /// <summary>
        /// Retorna a métrica de pontualidade do usuário autenticado.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<PunctualityMetricResponseDto>> GetMyMetric()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            var metric = await _service.GetMetricByUserId(user.Id);

            if (metric == null)
                return NotFound("Métricas ainda não calculadas.");

            return Ok(metric);
        }

        /// <summary>
        /// Exporta métricas de todas as empresas para classificação K-Means pela API Python.
        /// </summary>
        /// <remarks>
        /// Retorna apenas os atributos numéricos necessários para o algoritmo, com o CompanyId
        /// de cada registro para que a classificação seja feita por empresa.
        /// Autenticação exclusiva por API-key (header X-API-Key) — comunicação máquina-a-máquina.
        /// </remarks>
        [HttpGet("export")]
        [AllowAnonymous]
        [ApiKeyAuthorize]
        public async Task<ActionResult<List<PunctualityMetricExportDto>>> ExportForKMeans()
        {
            var metrics = await _service.ExportAllMetrics();
            return Ok(metrics);
        }

        /// <summary>
        /// Atualiza o cluster de um usuário após classificação K-Means.
        /// </summary>
        /// <remarks>
        /// Este endpoint é utilizado pela API Python para persistir o resultado da classificação.
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpPut("cluster")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> UpdateCluster([FromBody] UpdateClusterDto dto)
        {
            await _service.UpdateCluster(dto.UserId, dto.Cluster);
            return Ok("Cluster atualizado com sucesso.");
        }

        /// <summary>
        /// Atualiza clusters de múltiplos usuários em lote.
        /// </summary>
        /// <remarks>
        /// Este endpoint é utilizado pela API Python para persistir os resultados da classificação em lote.
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpPut("cluster/batch")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<IActionResult> UpdateClusterBatch([FromBody] UpdateClusterBatchDto dto)
        {
            await _service.UpdateClusterBatch(dto.Updates);
            return Ok("Clusters atualizados com sucesso.");
        }

        /// <summary>
        /// Atualiza clusters e centróides de todas as empresas após execução do K-Means.
        /// </summary>
        /// <remarks>
        /// Este endpoint é utilizado pela API Python para persistir os resultados da classificação
        /// e os centróides calculados por empresa, de forma consistente (mesma execução do modelo).
        /// Autenticação exclusiva por API-key (header X-API-Key) — comunicação máquina-a-máquina.
        /// </remarks>
        [HttpPut("classification")]
        [AllowAnonymous]
        [ApiKeyAuthorize]
        public async Task<IActionResult> UpdateClassification([FromBody] UpdateClassificationAllCompaniesDto dto)
        {
            await _service.UpdateClassificationForAllCompanies(dto);
            return Ok("Classificação atualizada com sucesso.");
        }

        /// <summary>
        /// Retorna dados para gráfico de dispersão.
        /// </summary>
        /// <remarks>
        /// Retorna pontos (usuários) e centróides para construção de gráfico de dispersão.
        /// Eixo X = late_percentage, Eixo Y = average_late_minutes, Cor = cluster.
        /// Disponível apenas para usuários com papel ADMIN ou RH.
        /// </remarks>
        [HttpGet("scatter-plot")]
        [Authorize(Policy = "RH_OR_ADMIN")]
        public async Task<ActionResult<ScatterPlotResponseDto>> GetScatterPlotData()
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            var data = await _service.GetScatterPlotData(user.CompanyId);
            return Ok(data);
        }
    }
}
