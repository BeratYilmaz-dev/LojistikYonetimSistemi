using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Models.Users;

namespace LogistikYonetimSistemi.Services;

/// <summary>
/// Protection Proxy: OrderFacade'i sarar; sahiplik ve rol kurallarını
/// servis katmanında zorlar. Controller'lar yetki mantığını bilmez.
/// </summary>
public class AuthorizedOrderFacade : IOrderFacade
{
    private readonly OrderFacade _inner;

    /// İptal: Admin ve Staff (Depo operasyonel iptal yapabilir).
    private static readonly HashSet<string> CancelPrivilegedRoles =
        new(StringComparer.OrdinalIgnoreCase) { "Admin", "Staff" };

    /// İade: yalnızca Admin. Müşteri sahiplik kontrolüyle zaten geçer.
    private static readonly HashSet<string> ReturnPrivilegedRoles =
        new(StringComparer.OrdinalIgnoreCase) { "Admin" };

    public AuthorizedOrderFacade(OrderFacade inner) => _inner = inner;

    public Order PlaceOrder(User customer, List<OrderItem> items,
                            PaymentType paymentType, CargoType cargoType,
                            bool addInsurance = false, bool addFragileProtection = false,
                            int distanceKm = 100)
        => _inner.PlaceOrder(customer, items, paymentType, cargoType,
                             addInsurance, addFragileProtection, distanceKm);

    public void ApproveOrder(string orderId)
        => _inner.ApproveOrder(orderId);

    public Order FindOrder(string orderId)
        => _inner.FindOrder(orderId);

    public IReadOnlyList<Order> GetAllOrders()
        => _inner.GetAllOrders();

    public void CancelOrder(string orderId, string callerUsername, string callerRole)
    {
        if (!IsAllowed(orderId, callerUsername, callerRole, CancelPrivilegedRoles))
            throw new UnauthorizedAccessException("Bu siparişi iptal etme yetkiniz yok.");

        _inner.CancelOrder(orderId, callerUsername, callerRole);
    }

    public void InitiateReturn(string orderId, string callerUsername, string callerRole)
    {
        if (!IsAllowed(orderId, callerUsername, callerRole, ReturnPrivilegedRoles))
            throw new UnauthorizedAccessException(
                "İade yalnızca sipariş sahibi müşteri veya Admin tarafından başlatılabilir.");

        _inner.InitiateReturn(orderId, callerUsername, callerRole);
    }

    private bool IsAllowed(string orderId, string callerUsername,
                           string callerRole, HashSet<string> privilegedRoles)
    {
        if (privilegedRoles.Contains(callerRole)) return true;

        var order = _inner.FindOrder(orderId);
        return order.Customer.Username.Equals(callerUsername, StringComparison.OrdinalIgnoreCase);
    }
}
