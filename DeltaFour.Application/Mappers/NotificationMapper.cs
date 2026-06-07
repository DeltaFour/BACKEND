using DeltaFour.Application.Dtos.Notifications;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class NotificationMapper
    {
        public static NotificationDto ToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Type = notification.Type.ToString(),
                Severity = notification.Severity.ToString(),
                Title = notification.Title,
                Message = notification.Message,
                UserId = notification.UserId,
                UserName = notification.User?.Name,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }

        public static List<NotificationDto> ToDtoList(IEnumerable<Notification> notifications)
        {
            return notifications.Select(ToDto).ToList();
        }
    }
}
