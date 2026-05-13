using BCrypt.Net;

namespace DeltaFour.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly int _workFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    public bool Verify(string password, string hashed)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashed);
    }
}
