namespace DeltaFour.Domain.Enum
{
    /// <summary>
    /// Severidade da notificação, usada pelo front para definir a cor do alerta.
    /// Info = azul, Success = verde, Warning = amarelo, Danger = vermelho.
    /// </summary>
    public enum NotificationSeverity
    {
        Info,
        Success,
        Warning,
        Danger
    }
}
