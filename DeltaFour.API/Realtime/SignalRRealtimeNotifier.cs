using DeltaFour.Application.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace DeltaFour.API.Realtime
{
    /// <summary>
    /// Implementação de <see cref="IRealtimeNotifier"/> baseada em SignalR.
    /// </summary>
    public class SignalRRealtimeNotifier(IHubContext<NotificationHub> hubContext) : IRealtimeNotifier
    {
        public Task NotifyCompanyAsync(Guid companyId, string eventName, object payload)
        {
            return hubContext.Clients
                .Group(NotificationHub.CompanyGroup(companyId))
                .SendAsync(eventName, payload);
        }
    }
}
