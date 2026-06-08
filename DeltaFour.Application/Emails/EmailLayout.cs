namespace DeltaFour.Application.Emails
{
    /// <summary>
    /// Layout base único e reutilizável dos e-mails: estrutura HTML responsiva,
    /// baseada em tabelas (compatível com Gmail, Outlook e Apple Mail), com
    /// cabeçalho (logo), título, área de conteúdo e rodapé padronizados.
    /// </summary>
    public static class EmailLayout
    {
        /// <summary>
        /// Envolve o conteúdo já renderizado no esqueleto visual completo.
        /// </summary>
        public static string Wrap(string title, string preheader, string contentHtml)
        {
            var year = DateTime.UtcNow.Year;
            var titleBlock = string.IsNullOrWhiteSpace(title)
                ? string.Empty
                : $"""
                   <h1 style="margin:0 0 20px;font-size:22px;line-height:1.3;font-weight:800;color:{EmailTheme.TextStrong};">{title}</h1>
                   """;

            return $$"""
                <!DOCTYPE html>
                <html lang="pt-BR" xmlns="http://www.w3.org/1999/xhtml">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
                  <meta name="color-scheme" content="light only" />
                  <title>{{EmailTheme.CompanyName}}</title>
                  <!--[if mso]>
                  <noscript><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml></noscript>
                  <![endif]-->
                  <style>
                    @media only screen and (max-width:600px){
                      .df-card{ width:100% !important; border-radius:0 !important; }
                      .df-pad{ padding-left:24px !important; padding-right:24px !important; }
                    }
                  </style>
                </head>
                <body style="margin:0;padding:0;background:{{EmailTheme.PageBg}};">
                  <div style="display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;">{{preheader}}</div>
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:{{EmailTheme.PageBg}};">
                    <tr>
                      <td align="center" style="padding:32px 16px;">
                        <table role="presentation" class="df-card" width="600" cellpadding="0" cellspacing="0"
                               style="width:600px;max-width:600px;background:{{EmailTheme.CardBg}};border:1px solid {{EmailTheme.Border}};border-radius:16px;overflow:hidden;box-shadow:0 8px 28px rgba(46,16,101,0.08);font-family:{{EmailTheme.FontFamily}};">

                          <!-- Faixa de marca -->
                          <tr>
                            <td style="height:6px;line-height:6px;font-size:0;background:{{EmailTheme.PrimaryDark}};background:linear-gradient(90deg,{{EmailTheme.PrimaryDark}} 0%,{{EmailTheme.Primary}} 50%,{{EmailTheme.Accent}} 100%);">&nbsp;</td>
                          </tr>

                          <!-- Cabeçalho com logo -->
                          <tr>
                            <td align="center" style="padding:32px 32px 8px;">
                              <img src="cid:{{EmailTheme.LogoContentId}}" width="172" alt="{{EmailTheme.CompanyName}}"
                                   style="display:block;width:172px;max-width:172px;height:auto;border:0;outline:none;text-decoration:none;" />
                            </td>
                          </tr>

                          <!-- Conteúdo -->
                          <tr>
                            <td class="df-pad" style="padding:24px 40px 8px;">
                              {{titleBlock}}
                              {{contentHtml}}
                            </td>
                          </tr>

                          <!-- Rodapé -->
                          <tr>
                            <td class="df-pad" style="padding:28px 40px 32px;background:{{EmailTheme.FooterBg}};border-top:1px solid {{EmailTheme.Border}};">
                              <p style="margin:0 0 6px;font-size:14px;font-weight:700;color:{{EmailTheme.TextStrong}};">{{EmailTheme.CompanyName}}</p>
                              <p style="margin:0 0 10px;font-size:12px;line-height:1.6;color:{{EmailTheme.TextMuted}};">{{EmailTheme.Tagline}}</p>
                              <p style="margin:0 0 4px;font-size:12px;line-height:1.6;color:{{EmailTheme.TextMuted}};">
                                Este é um e-mail automático enviado pelo sistema. Por favor, não responda diretamente a esta mensagem.
                              </p>
                              <p style="margin:0 0 4px;font-size:12px;line-height:1.6;color:{{EmailTheme.TextMuted}};">
                                Contato: <a href="mailto:{{EmailTheme.SupportEmail}}" style="color:{{EmailTheme.Accent}};text-decoration:none;">{{EmailTheme.SupportEmail}}</a>
                              </p>
                              <p style="margin:8px 0 0;font-size:11px;color:{{EmailTheme.TextMuted}};">&copy; {{year}} {{EmailTheme.CompanyName}}. Todos os direitos reservados.</p>
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                """;
        }
    }
}
