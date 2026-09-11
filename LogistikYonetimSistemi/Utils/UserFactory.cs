using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.ViewModels;

namespace LogistikYonetimSistemi.Utils;

/// <summary>
/// Factory Method: Rol türüne göre doğru User alt sınıfını üretir.
/// Dictionary tabanlı yaklaşım — yeni rol = yeni giriş, mevcut kod değişmez (OCP).
/// Controller somut sınıf adlarını bilmez (DIP).
/// </summary>
public static class UserFactory
{
    private static readonly Dictionary<UserRole, Func<CreateUserInput, string, User>> _builders =
        new()
        {
            [UserRole.Admin] = (i, h)
                => new Admin(i.Username, h),

            [UserRole.Staff] = (i, h)
                => new Staff(i.Username, h,
                             email:      i.Extra,
                             department: i.Extra2),

            [UserRole.Customer] = (i, h)
                => new Customer(i.Username, h,
                                email:   i.Extra,
                                address: i.Extra2),

            [UserRole.Courier] = (i, h)
                => new Courier(i.Username, h,
                               vehicleType: string.IsNullOrWhiteSpace(i.Extra)
                                                ? "Motosiklet"
                                                : i.Extra),
        };

    public static User Create(CreateUserInput input, string passwordHash)
    {
        if (!_builders.TryGetValue(input.Role, out var build))
            throw new ArgumentException($"Bilinmeyen kullanıcı rolü: {input.Role}");

        return build(input, passwordHash);
    }
}
