using LogistikYonetimSistemi.Models.Enums;

namespace LogistikYonetimSistemi.Models.Users;

public class Courier : User
{
    public string VehicleType { get; }  // Motosiklet, Araç, Bisiklet

    public Courier(string username, string passwordHash, string vehicleType = "Motosiklet")
        : base(username, passwordHash, UserRole.Courier)
    {
        VehicleType = vehicleType;
    }
}
