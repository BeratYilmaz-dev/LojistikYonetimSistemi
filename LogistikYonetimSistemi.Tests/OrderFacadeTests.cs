using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Services;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

/// <summary>
/// OrderFacade (Facade Pattern) entegrasyon testleri:
/// sipariş oluşturma, onaylama, iptal ve iade akışları.
/// </summary>
public class OrderFacadeTests
{
    // ── Yardımcılar ──────────────────────────────────────────────────────────
    private static (StockManager, OrderFacade, Customer, SimpleProduct) Setup(int initialStock = 20)
    {
        var stockManager = new StockManager();
        var product      = new SimpleProduct("CPU-F1", "Test CPU", 15_000, initialStock, 3);
        stockManager.AddProduct(product);

        var facade   = new OrderFacade(stockManager);
        var customer = new Customer("musteri1", "hash", "m@m.com", "İstanbul");
        return (stockManager, facade, customer, product);
    }

    private static List<OrderItem> Items(SimpleProduct p, int qty = 2)
        => new() { new OrderItem(p, qty) };

    // ── PlaceOrder ────────────────────────────────────────────────────────────
    [Fact]
    public void PlaceOrder_ReturnsOrder_InPendingState()
    {
        var (_, facade, customer, product) = Setup();

        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);

        Assert.NotNull(order);
        Assert.Equal("Beklemede", order.StateName);
    }

    [Fact]
    public void PlaceOrder_DeductsStockFromProduct()
    {
        var (stockManager, facade, customer, product) = Setup(20);

        facade.PlaceOrder(customer, Items(product, qty: 3), PaymentType.CreditCard, CargoType.Aras);

        Assert.Equal(17, stockManager.GetProduct("CPU-F1")!.Stock);
    }

    [Fact]
    public void PlaceOrder_AssignsTrackingNumber()
    {
        var (_, facade, customer, product) = Setup();

        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);

        Assert.False(string.IsNullOrWhiteSpace(order.TrackingNumber));
    }

    [Fact]
    public void PlaceOrder_OrderAppearsInGetAllOrders()
    {
        var (_, facade, customer, product) = Setup();

        facade.PlaceOrder(customer, Items(product), PaymentType.WireTransfer, CargoType.Yurtici);

        Assert.Single(facade.GetAllOrders());
    }

    // ── ApproveOrder ──────────────────────────────────────────────────────────
    [Fact]
    public void ApproveOrder_TransitionsToApproved()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);

        facade.ApproveOrder(order.Id);

        Assert.Equal("Onaylandı", order.StateName);
    }

    [Fact]
    public void ApproveOrder_AlreadyApproved_Throws()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);
        facade.ApproveOrder(order.Id);

        Assert.Throws<InvalidOperationException>(() => facade.ApproveOrder(order.Id));
    }

    // ── CancelOrder ───────────────────────────────────────────────────────────
    [Fact]
    public void CancelOrder_FromPending_TransitionsToCancelled()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);

        // OrderFacade doğrudan test edildiğinden callerUsername/Role gereklidir;
        // yetki kontrolü AuthorizedOrderFacade katmanında yapılır.
        facade.CancelOrder(order.Id, callerUsername: customer.Username, callerRole: "Customer");

        Assert.Equal("İptal", order.StateName);
    }

    [Fact]
    public void CancelOrder_FromInCargo_Throws()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);
        facade.ApproveOrder(order.Id);
        order.Prepare();
        order.ShipOut();

        Assert.Throws<InvalidOperationException>(
            () => facade.CancelOrder(order.Id, callerUsername: customer.Username, callerRole: "Customer"));
    }

    [Fact]
    public void FindOrder_NonExistent_ThrowsKeyNotFound()
    {
        var (_, facade, _, _) = Setup();
        Assert.Throws<KeyNotFoundException>(() => facade.FindOrder("YOK-ID"));
    }

    // ── InitiateReturn ────────────────────────────────────────────────────────
    [Fact]
    public void InitiateReturn_FromInCargo_RestoresStock()
    {
        var (stockManager, facade, customer, product) = Setup(20);
        var order = facade.PlaceOrder(customer, Items(product, qty: 3), PaymentType.CreditCard, CargoType.Aras);
        // 20 - 3 = 17
        Assert.Equal(17, stockManager.GetProduct("CPU-F1")!.Stock);

        facade.ApproveOrder(order.Id);
        order.Prepare(); order.ShipOut();
        facade.InitiateReturn(order.Id, callerUsername: customer.Username, callerRole: "Customer");

        // İade sonrası 17 + 3 = 20
        Assert.Equal(20, stockManager.GetProduct("CPU-F1")!.Stock);
    }

    [Fact]
    public void InitiateReturn_FromInCargo_SetsReturnState()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);
        facade.ApproveOrder(order.Id);
        order.Prepare(); order.ShipOut();

        facade.InitiateReturn(order.Id, callerUsername: customer.Username, callerRole: "Customer");

        Assert.Equal("İade", order.StateName);
    }

    [Fact]
    public void InitiateReturn_FromPending_Throws()
    {
        var (_, facade, customer, product) = Setup();
        var order = facade.PlaceOrder(customer, Items(product), PaymentType.CreditCard, CargoType.Aras);

        Assert.Throws<InvalidOperationException>(
            () => facade.InitiateReturn(order.Id, callerUsername: customer.Username, callerRole: "Customer"));
    }
}
