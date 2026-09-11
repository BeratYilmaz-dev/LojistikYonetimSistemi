using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Patterns.Notification;
using LogistikYonetimSistemi.Services;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

/// <summary>
/// StockManager'ın CRUD ve Observer (gözlemci) bildirimlerini test eder.
/// </summary>
public class StockManagerTests
{
    // ── Test Double: IStockObserver faking ──────────────────────────────────
    private class FakeObserver : IStockObserver
    {
        public List<(IProduct Product, int Stock)> Calls { get; } = new();

        public void OnStockLow(IProduct product, int currentStock)
            => Calls.Add((product, currentStock));
    }

    // ── Ürün yönetimi ────────────────────────────────────────────────────────
    [Fact]
    public void AddAndGet_Product_ReturnsCorrectProduct()
    {
        var manager = new StockManager();
        var product = new SimpleProduct("CPU-1", "Intel i9", 15000, 20, 5);

        manager.AddProduct(product);

        var result = manager.GetProduct("CPU-1");
        Assert.NotNull(result);
        Assert.Equal("Intel i9", result!.Name);
    }

    [Fact]
    public void GetProduct_NonExistent_ReturnsNull()
    {
        var manager = new StockManager();
        Assert.Null(manager.GetProduct("YOK-001"));
    }

    [Fact]
    public void RemoveProduct_RemovesFromDictionary()
    {
        var manager = new StockManager();
        manager.AddProduct(new SimpleProduct("GPU-1", "RTX 4090", 60000, 5, 2));

        manager.RemoveProduct("GPU-1");

        Assert.Null(manager.GetProduct("GPU-1"));
    }

    // ── Stok güncelleme ──────────────────────────────────────────────────────
    [Fact]
    public void UpdateStock_PositiveDelta_IncreasesStock()
    {
        var manager = new StockManager();
        manager.AddProduct(new SimpleProduct("RAM-1", "Corsair 32GB", 4000, 10, 5));

        manager.UpdateStock("RAM-1", +15);

        Assert.Equal(25, manager.GetProduct("RAM-1")!.Stock);
    }

    [Fact]
    public void UpdateStock_NegativeDelta_DecreasesStock()
    {
        var manager = new StockManager();
        manager.AddProduct(new SimpleProduct("SSD-1", "Samsung 990", 5000, 30, 8));

        manager.UpdateStock("SSD-1", -5);

        Assert.Equal(25, manager.GetProduct("SSD-1")!.Stock);
    }

    [Fact]
    public void UpdateStock_NonExistentProduct_ThrowsKeyNotFound()
    {
        var manager = new StockManager();
        Assert.Throws<KeyNotFoundException>(() => manager.UpdateStock("YOK-001", -1));
    }

    // ── Observer bildirimi ───────────────────────────────────────────────────
    [Fact]
    public void UpdateStock_BelowThreshold_NotifiesObserver()
    {
        var manager  = new StockManager();
        var observer = new FakeObserver();
        manager.RegisterObserver(observer);
        // Eşik: 5, başlangıç stok: 6 — bir azaltınca 5'e düşer (< 5 değil, = 5)
        // Eşik altı: stok < threshold
        manager.AddProduct(new SimpleProduct("MB-1", "ASUS ROG", 14000, 6, 6));

        manager.UpdateStock("MB-1", -1); // 6 → 5, eşik 6 olduğundan 5 < 6

        Assert.Single(observer.Calls);
        Assert.Equal(5, observer.Calls[0].Stock);
    }

    [Fact]
    public void UpdateStock_AboveThreshold_DoesNotNotifyObserver()
    {
        var manager  = new StockManager();
        var observer = new FakeObserver();
        manager.RegisterObserver(observer);
        manager.AddProduct(new SimpleProduct("PSU-1", "Corsair 1000W", 7000, 20, 5));

        manager.UpdateStock("PSU-1", -3); // 20 → 17, eşik 5, bildirim YOK

        Assert.Empty(observer.Calls);
    }

    [Fact]
    public void MultipleObservers_AllNotified_WhenBelowThreshold()
    {
        var manager = new StockManager();
        var obs1    = new FakeObserver();
        var obs2    = new FakeObserver();
        manager.RegisterObserver(obs1);
        manager.RegisterObserver(obs2);
        manager.AddProduct(new SimpleProduct("CASE-1", "NZXT H510", 3000, 4, 5));

        manager.UpdateStock("CASE-1", -1); // 4 → 3, eşik 5

        Assert.Single(obs1.Calls);
        Assert.Single(obs2.Calls);
    }

    [Fact]
    public void RemoveObserver_NotNotifiedAfterRemoval()
    {
        var manager  = new StockManager();
        var observer = new FakeObserver();
        manager.RegisterObserver(observer);
        manager.AddProduct(new SimpleProduct("COOL-1", "Noctua NH-D15", 3500, 4, 5));
        manager.RemoveObserver(observer);

        manager.UpdateStock("COOL-1", -1);

        Assert.Empty(observer.Calls);
    }
}
