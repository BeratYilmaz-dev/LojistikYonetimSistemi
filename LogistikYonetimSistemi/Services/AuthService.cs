using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Services;

public class AuthService
{
    private readonly UserService _userService;

    public AuthService(UserService userService) => _userService = userService;

    /// <summary>Kullanıcı adı ve şifreyi doğrular. Başarılıysa User nesnesini döner.</summary>
    public User? Login(string username, string password)
    {
        var user = _userService.FindByUsername(username);
        if (user == null || !user.VerifyPassword(password)) return null;
        Logger.Instance.Log($"Giriş başarılı: {username} ({user.Role})");
        return user;
    }

    public bool HasRole(User user, UserRole minimumRole) =>
        (int)user.Role <= (int)minimumRole;
}
