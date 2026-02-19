using DeltaFour.Application.RsaKeys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DeltaFour.CrossCutting.Ioc
{
    ///<sumary>
    ///Configuration for Jwt
    ///</sumary>
    public static class JwtConfiguration
    {
        public static IServiceCollection AddConfigJwt
        (
            this IServiceCollection service,
            IConfiguration configuration
        )
        {
            var rsaPublicKey = GetRsaKeys.GetPublicKey("../app.pub");
            bool validateLifeTime = bool.Parse(Environment.GetEnvironmentVariable("VALIDATE_LIFETIME")!);
            bool requireExpirationTime = bool.Parse(Environment.GetEnvironmentVariable("REQUIRE_EXPIRATION_TIME")!);
            bool validateIssuerSigningKey =
                bool.Parse(Environment.GetEnvironmentVariable("VALIDATE_ISSUER_SIGNING_KEY")!);
            bool issuer = bool.Parse(Environment.GetEnvironmentVariable("VALIDATE_ISSUER")!);
            bool audience = bool.Parse(Environment.GetEnvironmentVariable("VALIDATE_AUDIENCE")!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = issuer,
                ValidateAudience = audience,
                ValidateLifetime = validateLifeTime,
                RequireExpirationTime = requireExpirationTime,
                ValidateIssuerSigningKey = validateIssuerSigningKey,
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey),
                ClockSkew = TimeSpan.Zero,
            };
            service.AddSingleton(validationParameters);

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("Jwt"))
                        {
                            context.Token = context.Request.Cookies["Jwt"];
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Falha de autenticação: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

            return service;
        }
    }
}
