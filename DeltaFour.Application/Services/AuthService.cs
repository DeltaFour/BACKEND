using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Application.RsaKeys;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Domain.ValueObjects.Dtos;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace DeltaFour.Application.Services
{
    public class AuthService(IUnitOfWork repositories, IPasswordService passwordService) : IAuthService
    {
        private static readonly RSA PrivateKey = GetRsaKeys.GetPrivateKey("../app.key");
        private static readonly RSA PublicKey = GetRsaKeys.GetPublicKey("../app.pub");

        private readonly string host = Environment.GetEnvironmentVariable("EMAIL_HOST");
        private readonly int port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT"));
        private readonly string username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
        private readonly string emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        private readonly string fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL");
        private readonly string fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME");

        ///<summary>
        ///Operation for log user
        ///</summary>
        public virtual async Task<TreatedUserInformationDto?> Login(LoginDto dto)
        {
            var user = await repositories.UserRepository.FindUserInformation(dto.Email);

            if (user is { IsActive: true, IsConfirmed: true } && passwordService.Verify(dto.Password, user.Password!))
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
            var userAuth =
                await repositories.UserAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null && userAuth.IsExpired())
            {
                var user =
                    await repositories.UserRepository.FindIncludingRole(u => u.Id == GetUserIdFromToken(token)) ??
                    throw new BadHttpRequestException("Ops, algo deu errado");
                return CreateToken(AuthMapper.FromUserToTreatedUserInfo(user));
            }

            return null;
        }

        public async Task<Guid> RemakeRefreshToken(string token)
        {
            Guid userId = GetUserIdFromToken(token);
            return await CreateRefreshToken(userId, token);
        }

        ///<summary>
        ///Operation for logout user
        ///</summary>
        public async Task Logout(string refreshToken)
        {
            var userAuth =
                await repositories.UserAuthRepository.Find(ua => ua.Id == Guid.Parse(refreshToken));
            if (userAuth != null)
            {
                repositories.UserAuthRepository.Delete(userAuth);
                await repositories.Save();
            }
        }

        ///<summary>
        ///Altera a senha do usuário autenticado, exigindo a senha atual.
        ///</summary>
        public async Task ChangePassword(Guid userId, ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new BadHttpRequestException("A nova senha é obrigatória.");
            }

            var user = await repositories.UserRepository.Find(u => u.Id == userId)
                ?? throw new BadHttpRequestException("Usuário não encontrado.");

            if (!passwordService.Verify(dto.CurrentPassword, user.Password!))
            {
                throw new BadHttpRequestException("Senha atual incorreta.");
            }

            if (passwordService.Verify(dto.NewPassword, user.Password!))
            {
                throw new BadHttpRequestException("A nova senha deve ser diferente da senha atual.");
            }

            user.Password = passwordService.Hash(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            repositories.UserRepository.Update(user);
            await repositories.Save();
        }

        ///<summary>
        ///Gera um código de recuperação e envia para o e-mail do usuário (se existir).
        ///</summary>
        public async Task ForgotPassword(ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new BadHttpRequestException("O e-mail é obrigatório.");
            }

            var user = await repositories.UserRepository.Find(u => u.Email == dto.Email);

            // Não revela se o e-mail existe para evitar enumeração de usuários.
            if (user is not { IsActive: true })
            {
                return;
            }

            var code = GenerateResetCode();

            repositories.PasswordResetTokenRepository.Create(new PasswordResetToken
            {
                UserId = user.Id,
                Email = user.Email,
                Code = code,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30)
            });

            await repositories.Save();

            await SendPasswordResetEmailAsync(user.Email, code);
        }

        ///<summary>
        ///Redefine a senha a partir do código recebido por e-mail.
        ///</summary>
        public async Task ResetPassword(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new BadHttpRequestException("A nova senha é obrigatória.");
            }

            var user = await repositories.UserRepository.Find(u => u.Email == dto.Email)
                ?? throw new BadHttpRequestException("Código de recuperação inválido.");

            var resetToken = await repositories.PasswordResetTokenRepository.FindValid(user.Id, dto.Code)
                ?? throw new BadHttpRequestException("Código de recuperação inválido.");

            if (resetToken.ExpiresAtUtc < DateTime.UtcNow)
            {
                throw new BadHttpRequestException("O código de recuperação expirou.");
            }

            user.Password = passwordService.Hash(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            repositories.UserRepository.Update(user);

            resetToken.UsedAtUtc = DateTime.UtcNow;
            repositories.PasswordResetTokenRepository.Update(resetToken);

            await repositories.Save();
        }

        ///<summary>
        ///Gera um código numérico de 6 dígitos para recuperação de senha.
        ///</summary>
        private static string GenerateResetCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        ///<summary>
        ///Envia o código de recuperação de senha por e-mail.
        ///</summary>
        private async Task SendPasswordResetEmailAsync(string email, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Recuperação de senha";

            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                                <h2>Recuperação de senha</h2>

                                <p>Recebemos uma solicitação para redefinir a sua senha.</p>

                                <p>Utilize o código abaixo para concluir a redefinição:</p>

                                <p><strong>{code}</strong></p>

                                <p>Este código expira em 30 minutos. Se você não solicitou, ignore este e-mail.</p>
                            """
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        ///<summary>
        ///Define a senha no primeiro acesso do funcionário e desativa a flag MustChangePassword.
        ///</summary>
        public async Task SetInitialPassword(Guid userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new BadHttpRequestException("A nova senha é obrigatória.");
            }

            var user = await repositories.UserRepository.Find(u => u.Id == userId)
                ?? throw new BadHttpRequestException("Usuário não encontrado.");

            if (!user.MustChangePassword)
            {
                throw new BadHttpRequestException("Este usuário não está em modo de primeiro acesso.");
            }

            user.Password = passwordService.Hash(newPassword);
            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;
            repositories.UserRepository.Update(user);
            await repositories.Save();
        }

        ///<summary>
        ///Get id from token
        ///</summary>
        private Guid GetUserIdFromToken(string cookieToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(cookieToken);
            var user = JsonConvert.DeserializeObject<UserContext>(token.Claims.First().Value)!;
            return user.Id;
        }
    }

}
