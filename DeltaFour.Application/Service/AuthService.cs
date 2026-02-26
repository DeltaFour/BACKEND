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

        ///<summary>
        ///Operation for log user
        ///</summary>
        public async Task<TreatedUserInformationDto?> Login(LoginDto dto)
        {
            TreatedUserInformationDto? user =
                await repositories.UserRepository.FindUserInformation(dto.Email);
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

        ///<summary>
        ///Operation for mapUserInformation to response
        ///</summary>
        public UserInfoLoginDto MapUserInfo(TreatedUserInformationDto dto)
        {
            return AuthMapper.MapUserToUserInfoLoginDto(dto);
        }

        ///<summary>
        ///Create token for login
        ///</summary>
        public string CreateToken(TreatedUserInformationDto user)
        {
            var rsaPrivateKey = new RsaSecurityKey(PrivateKey);
            var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
            UserContext context = AuthMapper.UserContext(user);
            IDictionary<string, object> signingKeys = new Dictionary<string, object>()
            {
                { "user", JsonConvert.SerializeObject(context) },
                { "Role", user.RoleName }
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

        ///<summary>
        ///Create refresh token for loggin
        ///</summary>
        public async Task<Guid> CreateRefreshToken(Guid userId, string jwt)
        {
            UserAuth? userAuth = await repositories.UserAuthRepository.Find(u => u.UserId == userId);
            if (userAuth != null)
            {
                repositories.UserAuthRepository.Delete(userAuth);
            }
            var tokenHandler = new JsonWebTokenHandler();
            var rsaPublicKey = new RsaSecurityKey(PublicKey);
            var encryptingCredentials = new EncryptingCredentials(rsaPublicKey, SecurityAlgorithms.RsaOAEP,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            
            var tokenEncrypted = tokenHandler.EncryptToken(jwt, encryptingCredentials);

            userAuth = new UserAuth(userId, tokenEncrypted, DateTime.UtcNow.AddHours(24));
            repositories.UserAuthRepository.Create(userAuth);
            await repositories.Save();
            return userAuth.Id;
        }

        ///<summary>
        ///Remake the token of login
        ///</summary>
        public async Task<string?> RemakeToken(string refreshToken, string token)
        {
            UserAuth? userAuth =
                await repositories.UserAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null && userAuth.IsExpired())
            {
                User user =
                    await repositories.UserRepository.FindIncludingRole(u => u.Id == GetUserIdFromToken(token)) ??
                    throw new BadHttpRequestException("Ops, algo deu errado");
                return CreateToken(AuthMapper.FromUserToTreatedUserInfo(user));
            }

            return null;
        }

        public async Task<Guid> RemakeRefreshToken(string token)
        {
            Guid userId =  GetUserIdFromToken(token);
            return await CreateRefreshToken(userId, token);
        }

        ///<summary>
        ///Operation for logout user
        ///</summary>
        public async Task Logout(string refreshToken)
        {
            UserAuth? userAuth =
                await repositories.UserAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null)
            {
                repositories.UserAuthRepository.Delete(userAuth);
                await repositories.Save();
            }
        }

        ///<summary>
        ///Get id from token
        ///</summary>
        private Guid GetUserIdFromToken(string cookieToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(cookieToken);
            UserContext user = JsonConvert.DeserializeObject<UserContext>(token.Claims.First().Value)!;
            return user.Id;
        }
    }

}
