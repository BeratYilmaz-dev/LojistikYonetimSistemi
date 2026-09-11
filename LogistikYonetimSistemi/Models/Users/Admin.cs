using LogistikYonetimSistemi.Models.Enums;

namespace LogistikYonetimSistemi.Models.Users;

public class Admin : User
{
    public Admin(string username, string passwordHash)
        : base(username, passwordHash, UserRole.Admin) { }

    public void ManageUsers()  => Console.WriteLine($"[{Username}] kullanıcı yönetimi ekranı açıldı.");
    public void ViewAllLogs()  => Console.WriteLine($"[{Username}] tüm loglar görüntüleniyor.");
}
