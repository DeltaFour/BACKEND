namespace DeltaFour.Application.Services;

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hashed);
    string GenerateStrong(int length = 16);
}
