using LogistikYonetimSistemi.Models.Enums;
using System.Security.Cryptography;
using System.Text;

namespace LogistikYonetimSistemi.Models.Users;

public abstract class User
{
    public string   Id           { get; }
    public string   Username     { get; }
    private readonly string _passwordHash;
    public UserRole Role         { get; }

    protected User(string username, string passwordHash, UserRole role)
    {
        Id            = Guid.NewGuid().ToString("N")[..8].ToUpper();
        Username      = username;
        _passwordHash = passwordHash;
        Role          = role;
    }

    public bool VerifyPassword(string plainPassword)
        => HashPassword(plainPassword) == _passwordHash;

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + "_lys_salt"));
        return Convert.ToHexString(bytes);
    }

    public override string ToString() => $"[{Role}] {Username} ({Id})";
}
