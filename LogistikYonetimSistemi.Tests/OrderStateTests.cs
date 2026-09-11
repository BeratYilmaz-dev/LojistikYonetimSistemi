using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Models.Orders.States;
using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Utils;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

/// <summary>
/// Sipariş durum makinesinin (State Pattern) geçerli/geçersiz
/// geçişlerini test eder.
/// </summary>
public class OrderStateTests
{
    // ── Yardımcı ────────────────────────────────────────────────────────────
    private static Order CreateOrder()
    {
        var product = new SimpleProduct("CPU-T", "Test CPU", 5000, 10, 2);
        var items   = new List<OrderItem> { new(product, 1) };
        var payment = PaymentStrategyFactory.Create(PaymentType.CreditCard);
        var cargo   = CargoProviderFactory.Create(CargoType.Aras);
        return new Order(new Models.Users.Customer("test", "hash", "t@t.com", "Ankara"), items, payment, cargo);
    }

    // ── Başlangıç durumu ─────────────────────────────────────────────────────
    [Fact]
    public void NewOrder_StartsInPendingState()
    {
        var order = CreateOrder();
        Assert.Equal("Beklemede", order.StateName);
    }

    // ── Geçerli geçişler ──────────────────────────────────────────────────────
    [Fact]
    public void Approve_FromPending_TransitionsToApproved()
    {
        var order = CreateOrder();
        order.Approve();
        Assert.Equal("Onaylandı", order.StateName);
    }

    [Fact]
    public void Cancel_FromPending_TransitionsToCancelled()
    {
        var order = CreateOrder();
        order.Cancel();
        Assert.Equal("İptal", order.StateName);
    }

    [Fact]
    public void Cancel_FromApproved_TransitionsToCancelled()
    {
        var order = CreateOrder();
        order.Approve();
        order.Cancel();
        Assert.Equal("İptal", order.StateName);
    }

    [Fact]
    public void FullHappyPath_Pending_To_Delivered()
    {
        var order = CreateOrder();

        order.Approve();
        Assert.Equal("Onaylandı", order.StateName);

        order.Prepare();
        Assert.Equal("Hazırlanıyor", order.StateName);

        order.ShipOut();
        Assert.Equal("Kargoda", order.StateName);

        order.Deliver();
        Assert.Equal("Teslim Edildi", order.StateName);
    }

    [Fact]
    public void Return_FromInCargo_TransitionsToReturn()
    {
        var order = CreateOrder();
        order.Approve();
        order.Prepare();
        order.ShipOut();

        order.StartReturn();
        Assert.Equal("İade", order.StateName);
    }

    [Fact]
    public void Return_FromDelivered_TransitionsToReturn()
    {
        var order = CreateOrder();
        order.Approve();
        order.Prepare();
        order.ShipOut();
        order.Deliver();

        order.StartReturn();
        Assert.Equal("İade", order.StateName);
    }

    // ── Geçersiz geçişler (iş kuralı ihlalleri) ─────────────────────────────
    [Fact]
    public void Cancel_FromInCargo_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        order.Approve();
        order.Prepare();
        order.ShipOut();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void Prepare_FromPending_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        Assert.Throws<InvalidOperationException>(() => order.Prepare());
    }

    [Fact]
    public void Approve_FromApproved_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        order.Approve();
        Assert.Throws<InvalidOperationException>(() => order.Approve());
    }

    [Fact]
    public void ShipOut_FromApproved_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        order.Approve();
        // Hazırlanıyor'a geçmeden direkt Kargoya verilemiyor
        Assert.Throws<InvalidOperationException>(() => order.ShipOut());
    }

    [Fact]
    public void Cancel_FromDelivered_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        order.Approve(); order.Prepare(); order.ShipOut(); order.Deliver();
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void AnyAction_FromCancelled_ThrowsInvalidOperation()
    {
        var order = CreateOrder();
        order.Cancel();
        Assert.Throws<InvalidOperationException>(() => order.Approve());
    }
}
