namespace DeltaFour.Application.Emails
{
    /// <summary>
    /// Tokens visuais centralizados dos e-mails (cores, tipografia e identidade).
    /// Espelha a identidade do frontend (tema roxo "primary" e fonte Nunito).
    /// Altere aqui para refletir em todos os templates de uma só vez.
    /// </summary>
    public static class EmailTheme
    {
        // ── Marca / institucional ──────────────────────────────────────────
        public const string CompanyName = "DeltaFour";
        public const string Tagline = "Controle de ponto inteligente";

        /// <summary>E-mail de contato exibido no rodapé. Ajuste para o canal oficial de suporte.</summary>
        public const string SupportEmail = "deltafourponto@gmail.com";

        // ── Tipografia ─────────────────────────────────────────────────────
        // Nunito não é confiável em clientes de e-mail; usamos a mesma cadeia de fallback do frontend.
        public const string FontFamily =
            "Nunito, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif";

        // ── Paleta (espelha theme.ts: primary #4C1D95 + roxos do login) ────
        public const string Primary = "#4C1D95";       // primary.500
        public const string PrimaryDark = "#2E1065";   // topo do gradiente
        public const string Accent = "#7C3AED";        // realce/links
        public const string OnPrimary = "#FFFFFF";

        public const string TextStrong = "#1F2937";    // títulos
        public const string TextBody = "#374151";      // corpo
        public const string TextMuted = "#6B7280";     // secundário/rodapé

        public const string PageBg = "#F3F4F6";        // fundo da página
        public const string CardBg = "#FFFFFF";        // cartão central
        public const string FooterBg = "#FAF9FC";      // rodapé suave
        public const string Border = "#E9E7F0";        // bordas suaves
        public const string CodeBg = "#F5F3FF";        // caixa de código/destaque
        public const string CodeBorder = "#DDD6FE";

        // ── Identificador do logo embutido (CID) ───────────────────────────
        public const string LogoContentId = "deltafour-logo";
    }
}
