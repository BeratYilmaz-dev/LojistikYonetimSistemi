namespace LogistikYonetimSistemi.Models.Products;

public class CompositeProduct : IProduct
{
    private readonly List<IProduct> _components;

    public string Id        { get; }
    public string Name      { get; }
    public int    Threshold => 1;

    // Fiyat: tüm bileşenlerin toplamı
    public double Price    => _components.Sum(p => p.Price);
    // Ağırlık: tüm bileşenlerin toplamı (Composite Pattern — özyinelemeli)
    public double WeightKg => _components.Sum(p => p.WeightKg);

    // Stok: en az stoğa sahip bileşen kısıtlayıcıdır (montaj kapasitesi).
    // Setter: doğrudan çağrılmamalı; StockManager.UpdateStock her bileşeni
    // delta bazlı günceller. Setter yalnızca geriye dönük uyumluluk için tutuldu.
    public int Stock
    {
        get => _components.Count > 0 ? _components.Min(p => p.Stock) : 0;
        set => throw new InvalidOperationException(
            $"CompositeProduct stoğu doğrudan ayarlanamaz. " +
            $"Bileşenlerini ({string.Join(", ", _components.Select(c => c.Id))}) " +
            $"StockManager üzerinden güncelleyin.");
    }

    public CompositeProduct(string id, string name, List<IProduct> components)
    {
        Id           = id;
        Name         = name;
        _components  = components;
    }

    public void AddComponent(IProduct product)    => _components.Add(product);
    public void RemoveComponent(IProduct product) => _components.Remove(product);
    public IReadOnlyList<IProduct> GetComponents() => _components.AsReadOnly();

    public override string ToString() => $"[{Id}] {Name} — Fiyat: {Price:C}, Stok: {Stock} ({_components.Count} bileşen)";
}
