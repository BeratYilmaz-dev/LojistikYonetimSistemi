namespace LogistikYonetimSistemi.Models.Products;

public class SimpleProduct : IProduct
{
    public string Id        { get; }
    public string Name      { get; set; }
    public double Price     { get; set; }
    public double WeightKg  { get; set; }
    public int    Stock     { get; set; }
    public int    Threshold { get; set; }

    public SimpleProduct(string id, string name, double price, int stock, int threshold,
                         double weightKg = 0.5)
    {
        Id        = id;
        Name      = name;
        Price     = price;
        Stock     = stock;
        Threshold = threshold;
        WeightKg  = weightKg;
    }

    public override string ToString() => $"[{Id}] {Name} — {Price:C}, {WeightKg} kg, Stok: {Stock}";
}
