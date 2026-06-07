using DeltaFour.Application.Dtos;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Services;

public interface IAuthService
{
    Task<TreatedUserInformationDto?> Login(LoginDto dto);
    UserInfoLoginDto MapUserInfo(TreatedUserInformationDto dto);
    string CreateToken(TreatedUserInformationDto user);
    Task<Guid> CreateRefreshToken(Guid userId, string jwt);
    Task<string?> RemakeToken(string refreshToken, string token);
    Task<Guid> RemakeRefreshToken(string token);
    Task Logout(string refreshToken);

    /// <summary>
    /// Altera a senha do usuário autenticado, validando a senha atual.
    /// </summary>
    Task ChangePassword(Guid userId, ChangePasswordDto dto);

    /// <summary>
    /// Gera um código de recuperação de senha e o envia para o e-mail informado.
    /// </summary>
    Task ForgotPassword(ForgotPasswordDto dto);

    /// <summary>
    /// Redefine a senha a partir do código recebido por e-mail.
    /// </summary>
    Task ResetPassword(ResetPasswordDto dto);

    /// <summary>
    /// Define a senha inicial do funcionário no primeiro acesso.
    /// Limpa a flag MustChangePassword após a troca.
    /// </summary>
    Task SetInitialPassword(Guid userId, string newPassword);
}
