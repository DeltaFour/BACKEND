using DeltaFour.Application.Dtos.Notifications;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.Realtime;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using Serilog;

namespace DeltaFour.Application.Services
{
    /// <summary>
    /// Serviço central de notificações: persiste no banco (para sobreviver a reloads)
    /// e dispara em tempo real para a empresa. Reutilizável por outras features.
    /// </summary>
    public class NotificationService(IUnitOfWork unitOfWork, IRealtimeNotifier realtimeNotifier)
    {
        /// <summary>Nome do evento SignalR ouvido pelo front.</summary>
        public const string NotificationEvent = "ReceiveNotification";

        private const int RecentLimit = 50;

        /// <summary>
        /// Persiste a notificação e a envia em tempo real. O envio em tempo real é
        /// best-effort: uma falha de transporte não impede a gravação no banco.
        /// </summary>
        public async Task<NotificationDto> CreateAndDispatchAsync(Notification notification)
        {
            unitOfWork.NotificationRepository.Create(notification);
            await unitOfWork.Save();

            var dto = NotificationMapper.ToDto(notification);

            try
            {
                await realtimeNotifier.NotifyCompanyAsync(notification.CompanyId, NotificationEvent, dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Falha ao enviar notificação em tempo real para empresa {CompanyId}", notification.CompanyId);
            }

            return dto;
        }

        /// <summary>
        /// Cria e dispara a notificação de batida de ponto de entrada.
        /// Atraso = vermelho (Danger); no horário = azul (Info).
        /// </summary>
        public Task<NotificationDto> NotifyPunchInAsync(
            Guid companyId, Guid userId, string? userName, bool isLate, DateTime punchTime, Guid attendanceId)
        {
            var name = string.IsNullOrWhiteSpace(userName) ? "Funcionário" : userName.Trim();
            var time = punchTime.ToString("HH:mm");

            var notification = new Notification
            {
                CompanyId = companyId,
                UserId = userId,
                Type = NotificationType.PunchIn,
                Severity = isLate ? NotificationSeverity.Danger : NotificationSeverity.Info,
                Title = isLate ? "Entrada com atraso" : "Entrada registrada",
                Message = isLate
                    ? $"{name} bateu o ponto de entrada com atraso às {time}"
                    : $"{name} bateu o ponto de entrada às {time}",
                ReferenceId = attendanceId
            };

            return CreateAndDispatchAsync(notification);
        }

        /// <summary>Notificações recentes (últimas 24h) da empresa, mais novas primeiro.</summary>
        public async Task<List<NotificationDto>> GetRecentForCompany(Guid companyId)
        {
            var since = DateTime.UtcNow.AddDays(-1);
            var notifications = await unitOfWork.NotificationRepository
                .GetRecentByCompany(companyId, since, RecentLimit);

            return NotificationMapper.ToDtoList(notifications);
        }

        public Task<int> MarkAllAsRead(Guid companyId)
        {
            return unitOfWork.NotificationRepository.MarkAllAsRead(companyId);
        }
    }
}
