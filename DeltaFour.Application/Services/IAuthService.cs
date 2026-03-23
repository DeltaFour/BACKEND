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
}
