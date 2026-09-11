using LogistikYonetimSistemi.Models.Enums;

namespace LogistikYonetimSistemi.Models.Users;

public class Staff : User
{
    public string Email   { get; }
    public string Department { get; }

    public Staff(string username, string passwordHash, string email, string department)
        : base(username, passwordHash, UserRole.Staff)
    {
        Email = email;
        Department = department;
    }

    public void UpdateStock()   => Console.WriteLine($"[{Username}] stok güncelleme ekranı açıldı.");
    public void ProcessOrder()  => Console.WriteLine($"[{Username}] sipariş işleme ekranı açıldı.");
}
