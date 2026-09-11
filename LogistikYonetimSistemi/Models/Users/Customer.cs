using LogistikYonetimSistemi.Models.Enums;

namespace LogistikYonetimSistemi.Models.Users;

public class Customer : User
{
    public string Address { get; set; }
    public string Email   { get; }

    public Customer(string username, string passwordHash, string email, string address)
        : base(username, passwordHash, UserRole.Customer)
    {
        Email   = email;
        Address = address;
    }
}
