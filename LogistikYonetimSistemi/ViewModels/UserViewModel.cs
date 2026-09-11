using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Users;

namespace LogistikYonetimSistemi.ViewModels;

public class UserViewModel
{
    public string   Id       { get; init; } = "";
    public string   Username { get; init; } = "";
    public string   Role     { get; init; } = "";
    public string   Extra    { get; init; } = ""; // dept or email

    public static UserViewModel From(User u) => new()
    {
        Id       = u.Id,
        Username = u.Username,
        Role     = u.Role.ToString(),
        Extra    = u switch
        {
            Staff    s => s.Department,
            Customer c => c.Email,
            Courier  k => k.VehicleType,
            _          => ""
        }
    };
}

public class CreateUserInput
{
    public string   Username { get; set; } = "";
    public string   Password { get; set; } = "";
    public UserRole Role     { get; set; }
    /// <summary>Staff → E-posta | Customer → E-posta | Courier → Araç Tipi</summary>
    public string   Extra    { get; set; } = "";
    /// <summary>Staff → Departman | Customer → Adres | diğerleri → kullanılmaz</summary>
    public string   Extra2   { get; set; } = "";
}
