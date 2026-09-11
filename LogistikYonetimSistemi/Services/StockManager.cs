using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Patterns.Notification;
using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Services;

public class StockManager
{
    private readonly List<IStockObserver>          _observers = new();
    private readonly Dictionary<string, IProduct>  _products  = new();

    public void RegisterObserver(IStockObserver obs) => _observers.Add(obs);
    public void RemoveObserver(IStockObserver obs)   => _observers.Remove(obs);

    public void AddProduct(IProduct product)
        => _products[product.Id] = product;

    public IProduct? GetProduct(string productId)
        => _products.TryGetValue(productId, out var p) ? p : null;

    public IReadOnlyDictionary<string, IProduct> GetAllProducts()
        => _products;

    public void RemoveProduct(string productId)
        => _products.Remove(productId);

    /// <summary>
    /// CompositeProduct ise her bileşen ayrı ayrı güncellenir;
    /// bileşen stoklarının yanlışlıkla eşitlenmesi önlenir.
    /// </summary>
    public void UpdateStock(string productId, int delta)
    {
        if (!_products.TryGetValue(productId, out var product))
            throw new KeyNotFoundException($"Ürün bulunamadı: {productId}");

        if (product is CompositeProduct composite)
        {
            foreach (var component in composite.GetComponents())
                UpdateSingleProduct(component);
        }
        else
        {
            UpdateSingleProduct(product);
        }

        void UpdateSingleProduct(IProduct p)
        {
            int old = p.Stock;
            p.Stock += delta;
            Logger.Instance.LogStockChange(p.Id, old, p.Stock);
            CheckThreshold(p);
        }
    }

    private void CheckThreshold(IProduct product)
    {
        if (product.Stock <= product.Threshold)
            _observers.ForEach(obs => obs.OnStockLow(product, product.Stock));
    }
}
