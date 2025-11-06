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
            Employee? user = await repositories.EmployeeRepository.Find(u => u.Email == dto.Email);

            if (user is { IsActive: true, IsConfirmed: true } && user.Password == dto.Password)
            {
                return user;
            }

            return null;
        }

        public async Task<string> CreateToken(Employee employee)
        {
            var rsaPrivateKey = new RsaSecurityKey(PrivateKey);
            var rsaPublicKey = new RsaSecurityKey(PublicKey);
            var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
            var encryptingCredentials = new EncryptingCredentials(rsaPublicKey,SecurityAlgorithms.RsaOAEP,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            var role = await repositories.RoleRepository.Find(role => role.Id == employee.RoleId);
            IDictionary<string, object> signingKeys = new Dictionary<string, object>()
            {
                { "userId", employee.Id },
                { "CompanyId", employee.CompanyId },
                { "Role", role.Name }
            };
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Claims = signingKeys,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
            };
            
            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Guid> CreateRefreshToken(Employee employee, string jwt)
        {
            EmployeeAuth? userAuth = await repositories.EmployeeAuthRepository.Find(u=> u.EmployeeId == employee.Id);
            if (userAuth != null)
            {
                repositories.EmployeeAuthRepository.Delete(userAuth);
            }

            userAuth = new EmployeeAuth(employee.Id, jwt, DateTime.UtcNow.AddHours(24));
            repositories.EmployeeAuthRepository.Create(userAuth);
            await repositories.Save();
            return userAuth.Id;
        }

        public async Task<string?> RemakeToken(string refreshToken, string userId)
        {
            EmployeeAuth? userAuth = await repositories.EmployeeAuthRepository.Find(ua => ua.Id ==Guid.Parse(refreshToken));
            if (userAuth != null && userAuth.IsExpired())
            {
                Employee employee = await repositories.EmployeeRepository.Find(u => u.Id == Guid.Parse(userId)) ??
                            throw new BadHttpRequestException("Ops, algo deu errado");
                return await CreateToken(employee);
            }
            return null;
        }

        public async Task Logout(string refreshToken)
        {
            EmployeeAuth? userAuth = await repositories.EmployeeAuthRepository.Find(ua => ua.Id ==Guid.Parse(refreshToken));
            if (userAuth != null)
            {
                repositories.EmployeeAuthRepository.Delete(userAuth);
                await repositories.Save();
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
