using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        void Create(Notification notification);

        void Update(Notification notification);

        /// <summary>
        /// Retorna as notificações da empresa criadas a partir de <paramref name="since"/>,
        /// mais recentes primeiro, limitadas a <paramref name="limit"/>.
        /// </summary>
        Task<List<Notification>> GetRecentByCompany(Guid companyId, DateTime since, int limit);

        /// <summary>Marca como lidas todas as notificações não lidas da empresa.</summary>
        Task<int> MarkAllAsRead(Guid companyId);
    }
}
