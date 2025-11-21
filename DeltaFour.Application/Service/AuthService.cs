using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.RsaKeys;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace DeltaFour.Application.Service
{
    public class AuthService(AllRepositories repositories)
    {
        private static readonly RSA PrivateKey = GetRsaKeys.GetPrivateKey("../app.key");
        private static readonly RSA PublicKey = GetRsaKeys.GetPublicKey("../app.pub");

        public async Task<TreatedUserInformationDto?> Login(LoginDto dto)
        {
            TreatedUserInformationDto? user =
                await repositories.EmployeeRepository.FindUserInformation(dto.Email, dto.TimeLogged);
            using var hash = SHA256.Create();
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            var hashPassowrd = new StringBuilder();
            foreach (byte b in bytes)
            {
                hashPassowrd.Append(b.ToString("x2"));
            }
            if (user is { IsActive: true, IsConfirmed: true } && user.Password.Equals(hashPassowrd.ToString()))
            {
                return user;
            }

            return null;
        }

        public UserInfoLoginDto MapUserInfo(TreatedUserInformationDto dto)
        {
            return AuthMapper.MapUserToUserInfoLoginDto(dto);
        }

        public string CreateToken(TreatedUserInformationDto employee)
        {
            var rsaPrivateKey = new RsaSecurityKey(PrivateKey);
            var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
            UserContext context = AuthMapper.UserContext(employee);
            IDictionary<string, object> signingKeys = new Dictionary<string, object>()
            {
                { "user", JsonConvert.SerializeObject(context) },
                { "Role", employee.RoleName }
            };
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Claims = signingKeys,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = signingCredentials,
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Guid> CreateRefreshToken(TreatedUserInformationDto employee, string jwt)
        {
            EmployeeAuth? userAuth = await repositories.EmployeeAuthRepository.Find(u => u.EmployeeId == employee.Id);
            if (userAuth != null)
            {
                repositories.EmployeeAuthRepository.Delete(userAuth);
            }
            var tokenHandler = new JsonWebTokenHandler();
            var rsaPublicKey = new RsaSecurityKey(PublicKey);
            var encryptingCredentials = new EncryptingCredentials(rsaPublicKey, SecurityAlgorithms.RsaOAEP,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            
            var tokenEncrypted = tokenHandler.EncryptToken(jwt, encryptingCredentials);

            userAuth = new EmployeeAuth(employee.Id, tokenEncrypted, DateTime.UtcNow.AddHours(24));
            repositories.EmployeeAuthRepository.Create(userAuth);
            await repositories.Save();
            return userAuth.Id;
        }

        public async Task<string?> RemakeToken(string refreshToken, string token)
        {
            EmployeeAuth? userAuth =
                await repositories.EmployeeAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null && userAuth.IsExpired())
            {
                Employee employee =
                    await repositories.EmployeeRepository.FindIncludingRole(u => u.Id == GetUserIdFromToken(token)) ??
                    throw new BadHttpRequestException("Ops, algo deu errado");
                return CreateToken(AuthMapper.FromEmployeeToTreatedUserInfo(employee));
            }

            return null;
        }

        public async Task Logout(string refreshToken)
        {
            EmployeeAuth? userAuth =
                await repositories.EmployeeAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null)
            {
                repositories.EmployeeAuthRepository.Delete(userAuth);
                await repositories.Save();
            }
        }

        private Guid GetUserIdFromToken(string cookieToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(cookieToken);
            UserContext user = JsonConvert.DeserializeObject<UserContext>(token.Claims.First().Value)!;
            return user.Id;
        }
    }

}
