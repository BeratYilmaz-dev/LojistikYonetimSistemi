namespace LogistikYonetimSistemi.Models.Products;

public interface IProduct
{
    string Id        { get; }
    string Name      { get; }
    double Price     { get; }
    /// <summary>Brüt paket ağırlığı (kg). Bileşik ürünlerde bileşenlerin toplamıdır.</summary>
    double WeightKg  { get; }
    int    Stock     { get; set; }
    int    Threshold { get; }
}
