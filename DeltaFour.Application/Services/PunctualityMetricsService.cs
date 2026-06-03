using DeltaFour.Application.Dtos.PunctualityMetrics;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Services
{
    public class PunctualityMetricsService(IUnitOfWork unitOfWork)
    {
        /// <summary>
        /// Recalcula e persiste as métricas de pontualidade para um usuário específico.
        /// </summary>
        public async Task RecalculateMetricsForUser(Guid userId)
        {
            var attendances = await unitOfWork.UserAttendanceRepository
                .FindAll(a => a.UserId == userId && a.PunchType == PunchType.IN);

            var metric = await unitOfWork.UserPunctualityMetricRepository.GetByUserId(userId);
            bool isNew = metric == null;

            if (isNew)
            {
                metric = new UserPunctualityMetric
                {
                    UserId = userId
                };
            }

            int totalAttendances = attendances.Count;
            int totalLateAttendances = attendances.Count(a => a.IsLate);
            double latePercentage = totalAttendances > 0
                ? Math.Round((double)totalLateAttendances / totalAttendances * 100, 2)
                : 0;

            var lateMinutesList = attendances
                .Where(a => a.IsLate && a.TimeLate.HasValue)
                .Select(a => a.TimeLate!.Value.Hour * 60 + a.TimeLate.Value.Minute)
                .ToList();

            double averageLateMinutes = lateMinutesList.Count > 0
                ? Math.Round(lateMinutesList.Average(), 2)
                : 0;

            int maxLateMinutes = lateMinutesList.Count > 0
                ? lateMinutesList.Max()
                : 0;

            var workedDays = attendances
                .Select(a => a.PunchTime.Date)
                .Distinct()
                .Count();

            int totalAbsences = await CalculateAbsences(userId, attendances);

            metric.TotalAttendances = totalAttendances;
            metric.TotalLateAttendances = totalLateAttendances;
            metric.LatePercentage = latePercentage;
            metric.AverageLateMinutes = averageLateMinutes;
            metric.MaxLateMinutes = maxLateMinutes;
            metric.TotalWorkedDays = workedDays;
            metric.TotalAbsences = totalAbsences;
            metric.LastCalculatedAt = DateTime.UtcNow;
            metric.UpdatedAt = DateTime.UtcNow;

            if (isNew)
            {
                unitOfWork.UserPunctualityMetricRepository.Create(metric);
            }
            else
            {
                unitOfWork.UserPunctualityMetricRepository.Update(metric);
            }

            await unitOfWork.Save();
        }

        /// <summary>
        /// Calcula ausências baseado em dias úteis sem registro de ponto.
        /// </summary>
        private async Task<int> CalculateAbsences(Guid userId, List<UserAttendance> attendances)
        {
            if (attendances.Count == 0)
                return 0;

            var user = await unitOfWork.UserRepository.Find(u => u.Id == userId);
            if (user == null)
                return 0;

            var firstAttendance = attendances.Min(a => a.PunchTime.Date);
            var today = DateTime.UtcNow.Date;

            var workedDates = attendances
                .Select(a => a.PunchTime.Date)
                .Distinct()
                .ToHashSet();

            int absences = 0;
            for (var date = firstAttendance; date <= today; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday &&
                    date.DayOfWeek != DayOfWeek.Sunday &&
                    !workedDates.Contains(date))
                {
                    absences++;
                }
            }

            return absences;
        }

        /// <summary>
        /// Exporta métricas de todos os usuários de uma empresa para classificação K-Means.
        /// </summary>
        public async Task<List<PunctualityMetricExportDto>> ExportMetricsForCompany(Guid companyId)
        {
            var metrics = await unitOfWork.UserPunctualityMetricRepository.GetAllByCompanyId(companyId);

            return metrics.Select(m => new PunctualityMetricExportDto
            {
                UserId = m.UserId,
                LatePercentage = m.LatePercentage,
                AverageLateMinutes = m.AverageLateMinutes,
                MaxLateMinutes = m.MaxLateMinutes,
                TotalAbsences = m.TotalAbsences,
                TotalWorkedDays = m.TotalWorkedDays
            }).ToList();
        }

        /// <summary>
        /// Retorna métricas detalhadas de todos os usuários de uma empresa.
        /// </summary>
        public async Task<List<PunctualityMetricResponseDto>> GetMetricsByCompany(Guid companyId)
        {
            var metrics = await unitOfWork.UserPunctualityMetricRepository.GetAllByCompanyId(companyId);

            return metrics.Select(m => new PunctualityMetricResponseDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.Name,
                TotalAttendances = m.TotalAttendances,
                TotalLateAttendances = m.TotalLateAttendances,
                LatePercentage = m.LatePercentage,
                AverageLateMinutes = m.AverageLateMinutes,
                MaxLateMinutes = m.MaxLateMinutes,
                TotalAbsences = m.TotalAbsences,
                TotalWorkedDays = m.TotalWorkedDays,
                Cluster = m.Cluster,
                LastCalculatedAt = m.LastCalculatedAt
            }).ToList();
        }

        /// <summary>
        /// Atualiza o cluster de um usuário após classificação K-Means.
        /// </summary>
        public async Task UpdateCluster(Guid userId, int cluster)
        {
            await unitOfWork.UserPunctualityMetricRepository.UpdateCluster(userId, cluster);
        }

        /// <summary>
        /// Atualiza clusters de múltiplos usuários em lote.
        /// </summary>
        public async Task UpdateClusterBatch(List<UpdateClusterDto> updates)
        {
            var tuples = updates.Select(u => (u.UserId, u.Cluster)).ToList();
            await unitOfWork.UserPunctualityMetricRepository.UpdateClusterBatch(tuples);
        }

        /// <summary>
        /// Retorna a métrica de um usuário específico.
        /// </summary>
        public async Task<PunctualityMetricResponseDto?> GetMetricByUserId(Guid userId)
        {
            var metric = await unitOfWork.UserPunctualityMetricRepository.GetByUserId(userId);

            if (metric == null)
                return null;

            return new PunctualityMetricResponseDto
            {
                Id = metric.Id,
                UserId = metric.UserId,
                UserName = metric.User?.Name,
                TotalAttendances = metric.TotalAttendances,
                TotalLateAttendances = metric.TotalLateAttendances,
                LatePercentage = metric.LatePercentage,
                AverageLateMinutes = metric.AverageLateMinutes,
                MaxLateMinutes = metric.MaxLateMinutes,
                TotalAbsences = metric.TotalAbsences,
                TotalWorkedDays = metric.TotalWorkedDays,
                Cluster = metric.Cluster,
                LastCalculatedAt = metric.LastCalculatedAt
            };
        }

        /// <summary>
        /// Atualiza clusters e centróides de forma atômica após execução do K-Means.
        /// </summary>
        public async Task UpdateClustersWithCentroids(Guid companyId, UpdateClusterWithCentroidsDto dto)
        {
            // Atualiza clusters dos usuários
            var tuples = dto.UserClusters.Select(u => (u.UserId, u.Cluster)).ToList();
            await unitOfWork.UserPunctualityMetricRepository.UpdateClusterBatch(tuples);

            // Substitui centróides da empresa
            var centroids = dto.Centroids.Select(c => new ClusterCentroid
            {
                CompanyId = companyId,
                Cluster = c.Cluster,
                LatePercentage = c.LatePercentage,
                AverageLateMinutes = c.AverageLateMinutes,
                MaxLateMinutes = c.MaxLateMinutes,
                TotalAbsences = c.TotalAbsences,
                TotalWorkedDays = c.TotalWorkedDays,
                CalculatedAt = DateTime.UtcNow
            }).ToList();

            await unitOfWork.ClusterCentroidRepository.ReplaceForCompany(companyId, centroids);
        }

        /// <summary>
        /// Retorna dados para gráfico de dispersão.
        /// </summary>
        public async Task<ScatterPlotResponseDto> GetScatterPlotData(Guid companyId)
        {
            var metrics = await unitOfWork.UserPunctualityMetricRepository.GetAllByCompanyId(companyId);
            var centroids = await unitOfWork.ClusterCentroidRepository.GetByCompanyId(companyId);

            var response = new ScatterPlotResponseDto
            {
                Points = metrics.Select(m => new ScatterPlotPointDto
                {
                    UserId = m.UserId,
                    UserName = m.User?.Name,
                    LatePercentage = m.LatePercentage,
                    AverageLateMinutes = m.AverageLateMinutes,
                    Cluster = m.Cluster
                }).ToList(),

                Centroids = centroids.Select(c => new ScatterPlotCentroidDto
                {
                    Cluster = c.Cluster,
                    LatePercentage = c.LatePercentage,
                    AverageLateMinutes = c.AverageLateMinutes
                }).ToList()
            };

            return response;
        }
    }
}
