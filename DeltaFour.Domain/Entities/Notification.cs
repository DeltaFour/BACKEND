using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    /// <summary>
    /// Notificação persistida e enviada em tempo real para a dashboard da empresa.
    /// Estrutura genérica para suportar outros eventos de tempo real no futuro.
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>Empresa destinatária da notificação (escopo do broadcast).</summary>
        public Guid CompanyId { get; set; }

        /// <summary>Usuário relacionado ao evento (ex.: quem bateu o ponto).</summary>
        public Guid? UserId { get; set; }

        public NotificationType Type { get; set; }

        public NotificationSeverity Severity { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        /// <summary>Id da entidade de origem (ex.: UserAttendance), para rastreio.</summary>
        public Guid? ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public User? User { get; set; }
    }
}
