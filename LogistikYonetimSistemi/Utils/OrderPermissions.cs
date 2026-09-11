using System.Security.Claims;

namespace LogistikYonetimSistemi.Utils;

/// <summary>
/// Sipariş işlemleri için rol/departman yetki kurallarını merkezi bir yerde toplar.
/// Controller seviyesinde yetki kontrolleri yapılır.
/// </summary>
public static class OrderPermissions
{
    private const string DepoLabel = "Depo";

    /// Onayla (Beklemede → Onaylandı): Admin veya Depo personeli
    public static bool CanApprove(ClaimsPrincipal user)
        => user.IsInRole("Admin") || IsDepoStaff(user);

    /// Hazırla / Kargola: Admin, Kurye veya Depo personeli
    public static bool CanAdvance(ClaimsPrincipal user)
        => user.IsInRole("Admin") || user.IsInRole("Courier") || IsDepoStaff(user);

    /// Teslim Et: yalnızca Admin veya Kurye
    public static bool CanDeliver(ClaimsPrincipal user)
        => user.IsInRole("Admin") || user.IsInRole("Courier");

    /// Tüm siparişleri listeleme: Admin, Staff (her departman), Kurye
    public static bool CanSeeAllOrders(ClaimsPrincipal user)
        => user.IsInRole("Admin") || user.IsInRole("Staff") || user.IsInRole("Courier");

    private static bool IsDepoStaff(ClaimsPrincipal user)
        => user.IsInRole("Staff") &&
           user.FindFirst("Department")?.Value == DepoLabel;
}
