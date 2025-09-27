using DeltaFour.Application.Dtos;
using DeltaFour.Application.RsaKeys;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace DeltaFour.Application.Service
{
    public class AuthService(AllRepositories repositories)
    {
        private static readonly RSA PrivateKey = GetRsaKeys.GetPrivateKey("../app.key");
        private static readonly RSA PublicKey = GetRsaKeys.GetPublicKey("../app.pub");

        public async Task<Employee?> Login(LoginDto dto)
        {
            Employee? user = await repositories.UserRepository.Find(u => u.Email == dto.Email);
            if (user is { IsActive: true, IsConfirmed: true } && user.Password == dto.Password)
            {
                return user;
            }

            return null;
        }

        public string CreateToken(Employee employee)
        {
            var rsaPrivateKey = new RsaSecurityKey(PrivateKey);
            var rsaPublicKey = new RsaSecurityKey(PublicKey);
            var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
            var encryptingCredentials = new EncryptingCredentials(rsaPublicKey,SecurityAlgorithms.RsaOAEP,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            IDictionary<string, object> signingKeys = new Dictionary<string, object>()
            {
                { "userId", employee.Id },
                { "CompanyId", employee.CompanyId },

            };
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Claims = signingKeys,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = "http://localhost:5212",
                Audience = "http://localhost:5212",
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials
            };
            
            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Guid> CreateRefreshToken(Employee employee, string jwt)
        {
            EmployeeAuth? userAuth = await repositories.UserAuthRepository.Find(u=> u.EmployeeId == employee.Id);
            if (userAuth != null)
            {
                await repositories.UserAuthRepository.Delete(userAuth);
            }

            userAuth = new EmployeeAuth(employee.Id, jwt, DateTime.UtcNow.AddHours(24));
            await repositories.UserAuthRepository.Create(userAuth);
            return userAuth.Id;
        }

        public async Task<bool> CheckSession(string refreshToken, string jwt)
        {
            EmployeeAuth? userAuth = await repositories.UserAuthRepository.Find(ua => ua.Id ==Guid.Parse(refreshToken));
            if (userAuth != null && !userAuth.IsExpired())
            {

            }

            return false;
        }

        public async Task<string?> RemakeToken(string refreshToken, string userId)
        {
            EmployeeAuth? userAuth = await repositories.UserAuthRepository.Find(ua => ua.Id ==Guid.Parse(refreshToken));
            if (userAuth != null && userAuth.IsExpired())
            {
                Employee employee = await repositories.UserRepository.Find(u => u.Id == Guid.Parse(userId)) ??
                            throw new BadHttpRequestException("Ops, algo deu errado");
                return CreateToken(employee);
            }
            return null;
        }

        public async Task Logout(string refreshToken)
        {
            EmployeeAuth? userAuth = await repositories.UserAuthRepository.Find(ua => ua.Id ==Guid.Parse(refreshToken));
            if (userAuth != null)
            {
                await repositories.UserAuthRepository.Delete(userAuth);
            }
        }

        public static RSA GetPrivateKey(string caminho)
        {
            var rsa = RSA.Create();
            var primaryKey = File.ReadAllText(caminho);
            rsa.ImportFromPem(primaryKey);
            return rsa;
        }
    }

}
