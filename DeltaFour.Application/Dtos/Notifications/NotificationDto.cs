namespace DeltaFour.Application.Dtos.Notifications
{
    /// <summary>
    /// Representação de uma notificação enviada ao front (REST e SignalR).
    /// Type/Severity são strings para garantir o mesmo formato nos dois transportes.
    /// </summary>
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
