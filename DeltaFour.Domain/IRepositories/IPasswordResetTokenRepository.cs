using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface IPasswordResetTokenRepository
{
    /// <summary>
    /// Retorna o código de recuperação mais recente, ainda não utilizado, para o usuário e código informados.
    /// </summary>
    Task<PasswordResetToken?> FindValid(Guid userId, string code);

    void Create(PasswordResetToken token);

    void Update(PasswordResetToken token);
}
