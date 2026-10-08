using System.Security.Cryptography;

namespace PasswordGenerator.Domain.Services;

public sealed class PasswordGeneratorService
{
    private const int PasswordLength = 16;
    private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
    private const string DigitCharacters = "0123456789";
    private const string SpecialCharacters = "!@#$%&*-_+=";
    private const string AllCharacters =
        UppercaseCharacters + LowercaseCharacters + DigitCharacters + SpecialCharacters;

    public string GeneratePassword()
    {
        char[] password = new char[PasswordLength];

        password[0] = GetRandomCharacter(UppercaseCharacters);
        password[1] = GetRandomCharacter(LowercaseCharacters);
        password[2] = GetRandomCharacter(DigitCharacters);
        password[3] = GetRandomCharacter(SpecialCharacters);

        for (int i = 4; i < password.Length; i++)
        {
            password[i] = GetRandomCharacter(AllCharacters);
        }

        for (int i = password.Length - 1; i > 0; i--)
        {
            int swapIndex = RandomNumberGenerator.GetInt32(i + 1);
            (password[i], password[swapIndex]) = (password[swapIndex], password[i]);
        }

        return new string(password);
    }

    private static char GetRandomCharacter(string characters)
    {
        return characters[RandomNumberGenerator.GetInt32(characters.Length)];
    }
}
