using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Models.Users;

namespace LogistikYonetimSistemi.Services;

/// <summary>
/// Sipariş işlemlerinin soyutlaması.
/// OrderFacade gerçek uygulamayı, AuthorizedOrderFacade koruma proxy'sini sağlar.
/// </summary>
public interface IOrderFacade
{
    Order PlaceOrder(User customer, List<OrderItem> items,
                     PaymentType paymentType, CargoType cargoType,
                     bool addInsurance = false, bool addFragileProtection = false,
                     int distanceKm = 100);

    void ApproveOrder(string orderId);

    /// <param name="callerUsername">İşlemi yapan kullanıcının adı.</param>
    /// <param name="callerRole">İşlemi yapan kullanıcının rolü.</param>
    void CancelOrder(string orderId, string callerUsername, string callerRole);

    /// <param name="callerUsername">İşlemi yapan kullanıcının adı.</param>
    /// <param name="callerRole">İşlemi yapan kullanıcının rolü.</param>
    void InitiateReturn(string orderId, string callerUsername, string callerRole);

    Order FindOrder(string orderId);

    IReadOnlyList<Order> GetAllOrders();
}
