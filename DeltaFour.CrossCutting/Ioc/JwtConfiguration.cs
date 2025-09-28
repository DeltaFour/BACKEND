using DeltaFour.Application.RsaKeys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace DeltaFour.CrossCutting.Ioc
{
    public static class JwtConfiguration
    {
        
        public static IServiceCollection AddConfigJwt
        (
            this IServiceCollection service,
            IConfiguration configuration
        )
        {
            var rsaPublicKey = GetRsaKeys.GetPublicKey("../app.pub");
            var rsaPrivateKey = GetRsaKeys.GetPrivateKey("../app.key");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://localhost:5212",
                ValidAudience = "http://localhost:5212",
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey),
                TokenDecryptionKey = new RsaSecurityKey(rsaPrivateKey)
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
                    }
                };
            });
            return service;
        }
    }
}
