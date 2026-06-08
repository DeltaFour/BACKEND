namespace DeltaFour.Application.Emails
{
    /// <summary>
    /// Abstração única para envio de e-mails transacionais.
    /// Centraliza a configuração SMTP e a montagem da mensagem (incluindo o logo
    /// embutido), evitando duplicação de código entre os serviços.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>Envia um e-mail HTML para um único destinatário.</summary>
        Task SendAsync(string to, string subject, string htmlBody);

        /// <summary>Envia um e-mail HTML para múltiplos destinatários.</summary>
        Task SendAsync(IEnumerable<string> to, string subject, string htmlBody);
    }
}
