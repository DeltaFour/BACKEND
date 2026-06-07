namespace DeltaFour.Application.Realtime
{
    /// <summary>
    /// Abstração de envio em tempo real (implementada via SignalR na camada de API).
    /// Mantida na Application para que os serviços de domínio permaneçam desacoplados
    /// do transporte. Reutilizável por qualquer feature de tempo real do sistema.
    /// </summary>
    public interface IRealtimeNotifier
    {
        /// <summary>
        /// Envia um evento para todos os clientes conectados de uma empresa.
        /// </summary>
        Task NotifyCompanyAsync(Guid companyId, string eventName, object payload);
    }
}
