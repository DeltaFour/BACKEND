using System.Security.Cryptography;
using BCrypt.Net;

namespace DeltaFour.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly int _workFactor = 12;

    // Conjuntos sem caracteres ambíguos (l, I, O, 0, 1) para facilitar a leitura pelo usuário.
    private const string LowerChars = "abcdefghijkmnopqrstuvwxyz";
    private const string UpperChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string DigitChars = "23456789";
    private const string SpecialChars = "!@#$%*?-_";

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    public bool Verify(string password, string hashed)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashed);
    }

    ///<summary>
    /// Gera uma senha forte e aleatória (criptograficamente segura) garantindo ao menos
    /// uma letra minúscula, uma maiúscula, um dígito e um caractere especial, sem espaços.
    ///</summary>
    public string GenerateStrong(int length = 16)
    {
        if (length < 8)
        {
            length = 8;
        }

        var all = LowerChars + UpperChars + DigitChars + SpecialChars;
        var chars = new char[length];

        // Garante a presença de cada categoria exigida pela política de senha.
        chars[0] = LowerChars[RandomNumberGenerator.GetInt32(LowerChars.Length)];
        chars[1] = UpperChars[RandomNumberGenerator.GetInt32(UpperChars.Length)];
        chars[2] = DigitChars[RandomNumberGenerator.GetInt32(DigitChars.Length)];
        chars[3] = SpecialChars[RandomNumberGenerator.GetInt32(SpecialChars.Length)];

        for (int i = 4; i < length; i++)
        {
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        // Embaralhamento Fisher-Yates para não fixar a posição das categorias garantidas.
        for (int i = length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }
}
