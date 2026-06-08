using System.Net;
using System.Text;

namespace DeltaFour.Application.Emails
{
    /// <summary>
    /// Construtor fluente de e-mails. Cada método adiciona um bloco de conteúdo
    /// padronizado; <see cref="Render"/> envolve tudo no layout base reutilizável
    /// (cabeçalho com logo, título, conteúdo, rodapé), garantindo identidade visual
    /// única e compatibilidade com os principais clientes de e-mail.
    /// </summary>
    public sealed class EmailBuilder
    {
        private readonly StringBuilder _content = new();
        private string _title = string.Empty;
        private string _preheader = string.Empty;

        public static EmailBuilder Create() => new();

        /// <summary>Texto de pré-visualização exibido na caixa de entrada (oculto no corpo).</summary>
        public EmailBuilder Preheader(string text)
        {
            _preheader = Encode(text);
            return this;
        }

        /// <summary>Título principal destacado do e-mail.</summary>
        public EmailBuilder Title(string title)
        {
            _title = Encode(title);
            return this;
        }

        /// <summary>Parágrafo de texto comum.</summary>
        public EmailBuilder Paragraph(string text)
        {
            _content.Append(
                $"""
                 <p style="margin:0 0 16px;font-size:15px;line-height:1.6;color:{EmailTheme.TextBody};">{Encode(text)}</p>
                 """);
            return this;
        }

        /// <summary>Caixa de destaque para códigos (recuperação, assinatura, etc.).</summary>
        public EmailBuilder CodeBox(string code, string? caption = null)
        {
            if (!string.IsNullOrWhiteSpace(caption))
            {
                _content.Append(
                    $"""
                     <p style="margin:0 0 8px;font-size:13px;color:{EmailTheme.TextMuted};text-align:center;">{Encode(caption)}</p>
                     """);
            }

            _content.Append(
                $"""
                 <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:8px 0 24px;">
                   <tr>
                     <td align="center">
                       <div style="display:inline-block;background:{EmailTheme.CodeBg};border:1px solid {EmailTheme.CodeBorder};border-radius:12px;padding:18px 32px;">
                         <span style="font-family:'Courier New',Consolas,monospace;font-size:30px;font-weight:700;letter-spacing:6px;color:{EmailTheme.Primary};">{Encode(code)}</span>
                       </div>
                     </td>
                   </tr>
                 </table>
                 """);
            return this;
        }

        /// <summary>Linha de informação rotulada (label + valor), estilo "cartão de dados".</summary>
        public EmailBuilder InfoRow(string label, string value)
        {
            _content.Append(
                $"""
                 <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:0 0 10px;background:{EmailTheme.CodeBg};border:1px solid {EmailTheme.Border};border-radius:10px;">
                   <tr>
                     <td style="padding:12px 16px;">
                       <span style="display:block;font-size:12px;text-transform:uppercase;letter-spacing:.5px;color:{EmailTheme.TextMuted};margin-bottom:2px;">{Encode(label)}</span>
                       <span style="display:block;font-size:15px;font-weight:600;color:{EmailTheme.TextStrong};">{Encode(value)}</span>
                     </td>
                   </tr>
                 </table>
                 """);
            return this;
        }

        /// <summary>Botão de ação principal (call to action).</summary>
        public EmailBuilder Button(string text, string url)
        {
            var safeUrl = EncodeAttribute(url);
            _content.Append(
                $"""
                 <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:24px 0;">
                   <tr>
                     <td align="center">
                       <a href="{safeUrl}" target="_blank"
                          style="display:inline-block;background:{EmailTheme.Primary};color:{EmailTheme.OnPrimary};
                                 font-size:15px;font-weight:700;text-decoration:none;padding:14px 36px;border-radius:10px;
                                 box-shadow:0 4px 14px rgba(76,29,149,0.30);">
                         {Encode(text)}
                       </a>
                     </td>
                   </tr>
                 </table>
                 """);
            return this;
        }

        /// <summary>Link de fallback textual (quando há botão, mostra a URL acessível).</summary>
        public EmailBuilder LinkFallback(string url, string? prefix = "Ou acesse diretamente:")
        {
            var safeUrl = EncodeAttribute(url);
            _content.Append(
                $"""
                 <p style="margin:0 0 16px;font-size:13px;line-height:1.6;color:{EmailTheme.TextMuted};word-break:break-all;text-align:center;">
                   {Encode(prefix ?? string.Empty)}<br/>
                   <a href="{safeUrl}" target="_blank" style="color:{EmailTheme.Accent};">{Encode(url)}</a>
                 </p>
                 """);
            return this;
        }

        /// <summary>Aviso/nota de segurança em destaque suave.</summary>
        public EmailBuilder Note(string text)
        {
            _content.Append(
                $"""
                 <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:8px 0 4px;">
                   <tr>
                     <td style="padding:12px 16px;background:{EmailTheme.CodeBg};border-left:3px solid {EmailTheme.Accent};border-radius:6px;">
                       <span style="font-size:13px;line-height:1.6;color:{EmailTheme.TextMuted};">{Encode(text)}</span>
                     </td>
                   </tr>
                 </table>
                 """);
            return this;
        }

        /// <summary>Espaçador/divisória sutil.</summary>
        public EmailBuilder Divider()
        {
            _content.Append(
                $"""<div style="height:1px;background:{EmailTheme.Border};margin:24px 0;"></div>""");
            return this;
        }

        /// <summary>Renderiza o HTML final completo envolvendo o conteúdo no layout base.</summary>
        public string Render() => EmailLayout.Wrap(_title, _preheader, _content.ToString());

        private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

        private static string EncodeAttribute(string? value) =>
            WebUtility.HtmlEncode(value ?? string.Empty).Replace("\"", "&quot;");
    }
}
