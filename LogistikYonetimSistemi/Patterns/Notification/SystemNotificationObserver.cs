using LogistikYonetimSistemi.Models.Products;

namespace LogistikYonetimSistemi.Patterns.Notification;

public class SystemNotificationObserver : IStockObserver
{
    private readonly string _targetUserId;

    public SystemNotificationObserver(string targetUserId) => _targetUserId = targetUserId;

    public void OnStockLow(IProduct product, int currentStock)
    {
        Console.WriteLine($"  [SİSTEM BİLDİRİMİ → {_targetUserId}] Kritik stok uyarısı: " +
                          $"'{product.Name}' ({product.Id}) — Mevcut: {currentStock}, Eşik: {product.Threshold}");
    }
}
