namespace DeltaFour.Application.Common
{
    /// <summary>
    /// Centraliza o fuso horário da aplicação para conversões entre UTC e o
    /// horário local. Os registros são gravados em UTC; as regras de negócio que
    /// dependem do horário "de parede" (ex.: validação de atraso na jornada)
    /// devem comparar usando o horário local retornado por esta classe.
    ///
    /// O fuso pode ser configurado via variável de ambiente APP_TIMEZONE
    /// (padrão: America/Sao_Paulo).
    /// </summary>
    public static class AppClock
    {
        private static readonly TimeZoneInfo TimeZone = ResolveTimeZone();

        private static TimeZoneInfo ResolveTimeZone()
        {
            var id = Environment.GetEnvironmentVariable("APP_TIMEZONE");

            foreach (var candidate in new[] { id, "America/Sao_Paulo", "E. South America Standard Time" })
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(candidate);
                }
                catch (TimeZoneNotFoundException)
                {
                    // tenta o próximo identificador
                }
                catch (InvalidTimeZoneException)
                {
                    // tenta o próximo identificador
                }
            }

            // Fallback: horário de Brasília fixo (UTC-3, sem horário de verão desde 2019).
            return TimeZoneInfo.CreateCustomTimeZone(
                "BRT", TimeSpan.FromHours(-3), "Horário de Brasília", "BRT");
        }

        /// <summary>
        /// Converte um instante para o horário local da aplicação.
        /// Valores Unspecified são tratados como UTC (convenção do projeto).
        /// </summary>
        public static DateTime ToLocal(DateTime value)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            };

            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZone);
        }

        /// <summary>
        /// Horário local atual da aplicação.
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
    }
}
