using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DeltaFour.API.Realtime
{
    /// <summary>
    /// Hub SignalR de tempo real. Cada conexão entra no grupo da sua empresa, de modo
    /// que um broadcast atinge apenas os usuários daquela empresa. Genérico o suficiente
    /// para servir outras features de tempo real no futuro.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        private const string CompanyGroupPrefix = "company-";

        public static string CompanyGroup(Guid companyId) => $"{CompanyGroupPrefix}{companyId}";

        public override async Task OnConnectedAsync()
        {
            var companyId = GetCompanyId();
            if (companyId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, CompanyGroup(companyId.Value));
            }

            await base.OnConnectedAsync();
        }

        // O SignalR remove a conexão dos grupos automaticamente ao desconectar.

        private Guid? GetCompanyId()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                if (httpContext is null)
                {
                    return null;
                }

                var user = httpContext.GetUserAuthenticated<UserContext>();
                return user.CompanyId;
            }
            catch
            {
                return null;
            }
        }
    }
}
