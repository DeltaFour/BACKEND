using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DeltaFour.Application.Emails
{
    /// <summary>
    /// Implementação de <see cref="IEmailSender"/> usando MailKit/MimeKit.
    /// Lê a configuração SMTP do ambiente (ponto único) e embute o logo da marca
    /// como recurso vinculado (CID), garantindo renderização confiável da imagem.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly string _host = Environment.GetEnvironmentVariable("EMAIL_HOST")!;
        private readonly int _port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!);
        private readonly string _username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")!;
        private readonly string _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!;
        private readonly string _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL")!;
        private readonly string _fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME")!;

        // Bytes do logo embutido, carregados uma única vez.
        private static readonly byte[] LogoBytes = LoadLogo();

        public Task SendAsync(string to, string subject, string htmlBody) =>
            SendAsync(new[] { to }, subject, htmlBody);

        public async Task SendAsync(IEnumerable<string> to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));

            foreach (var recipient in to)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }
            }

            message.Subject = subject;
            message.Body = BuildBody(htmlBody);

            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private static MimeEntity BuildBody(string htmlBody)
        {
            var builder = new BodyBuilder { HtmlBody = htmlBody };

            if (LogoBytes.Length > 0)
            {
                var logo = builder.LinkedResources.Add("deltafour-logo.png", LogoBytes);
                logo.ContentId = EmailTheme.LogoContentId;
                logo.ContentType.MediaType = "image";
                logo.ContentType.MediaSubtype = "png";
            }

            return builder.ToMessageBody();
        }

        private static byte[] LoadLogo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("deltafour-logo.png", StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                return Array.Empty<byte>();
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                return Array.Empty<byte>();
            }

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
