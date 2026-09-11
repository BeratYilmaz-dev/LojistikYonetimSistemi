using LogistikYonetimSistemi.Models.Products;

namespace LogistikYonetimSistemi.Patterns.Notification;

public interface IStockObserver
{
    void OnStockLow(IProduct product, int currentStock);
}
