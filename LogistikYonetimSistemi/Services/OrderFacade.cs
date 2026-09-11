using LogistikYonetimSistemi.Exceptions;
using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Patterns.Fee;
using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Services;

public class OrderFacade : IOrderFacade
{
    private readonly StockManager _stockManager;
    private readonly List<Order>  _orders = new();
    private readonly Logger       _logger = Logger.Instance;

    public OrderFacade(StockManager stockManager)
    {
        _stockManager = stockManager;
    }

    /// Kargo ücreti Decorator zinciriyle hesaplanır; ödeme alınır; stok rezerve edilir.
    public Order PlaceOrder(User customer, List<OrderItem> items,
                            PaymentType paymentType, CargoType cargoType,
                            bool addInsurance = false, bool addFragileProtection = false,
                            int distanceKm = 100)
    {
        var payment = PaymentStrategyFactory.Create(paymentType);
        var cargo   = CargoProviderFactory.Create(cargoType);

        double weightKg = items.Sum(i => i.Product.WeightKg * i.Quantity);
        IFeeCalculator feeCalc = new BaseCargoFee();
        if (addInsurance)         feeCalc = new InsuranceDecorator(feeCalc);
        if (addFragileProtection) feeCalc = new FragileProtectionDecorator(feeCalc);
        double cargoFee = feeCalc.Calculate(weightKg, distanceKm);

        var order = new Order(customer, items, payment, cargo);
        order.SetCargoFee(cargoFee, addInsurance, addFragileProtection);

        bool paid = payment.Pay(order.TotalAmount);
        if (!paid) throw new PaymentFailedException("Ödeme başarısız oldu.");

        _logger.LogPayment(order.Id, payment.MethodName, order.TotalAmount);
        _logger.Log($"Kargo ücreti [{order.Id}]: {cargoFee:N0}₺" +
                    $"{(addInsurance ? " + Sigorta" : "")}" +
                    $"{(addFragileProtection ? " + Kırılgan Koruma" : "")}");

        foreach (var item in items)
            _stockManager.UpdateStock(item.Product.Id, -item.Quantity);

        order.TrackingNumber = cargo.GenerateTrackingNumber();
        _logger.Log($"Sipariş oluşturuldu [{order.Id}], takip: {order.TrackingNumber}");

        _orders.Add(order);
        return order;
    }

    /// LogStateChange, State sınıfı içinde çağrıldığı için burada tekrar çağrılmaz.
    public void ApproveOrder(string orderId)
    {
        var order = FindOrder(orderId);
        order.Approve();
    }

    public void CancelOrder(string orderId, string callerUsername, string callerRole)
    {
        var order = FindOrder(orderId);
        var prev  = order.StateName;
        order.Cancel();
        _logger.LogStateChange(orderId, prev, "İptal");
    }

    /// İade başlatılınca stoklar geri yüklenir.
    public void InitiateReturn(string orderId, string callerUsername, string callerRole)
    {
        var order = FindOrder(orderId);
        var prev  = order.StateName;
        order.StartReturn();
        _logger.LogStateChange(orderId, prev, "İade");

        foreach (var item in order.Items)
        {
            _stockManager.UpdateStock(item.Product.Id, +item.Quantity);
            _logger.Log($"İade stok iadesi — Ürün: {item.Product.Id}, Miktar: +{item.Quantity}");
        }
    }

    public Order FindOrder(string orderId)
        => _orders.FirstOrDefault(o => o.Id == orderId)
           ?? throw new KeyNotFoundException($"Sipariş bulunamadı: {orderId}");

    public IReadOnlyList<Order> GetAllOrders() => _orders.AsReadOnly();
}
