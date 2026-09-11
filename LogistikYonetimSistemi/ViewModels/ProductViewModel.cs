using LogistikYonetimSistemi.Models.Products;

namespace LogistikYonetimSistemi.ViewModels;

public class ProductViewModel
{
    public string Id             { get; init; } = "";
    public string Name           { get; init; } = "";
    public string Category       { get; init; } = "";
    public double Price          { get; init; }
    public double WeightKg       { get; init; }
    public int    Stock          { get; init; }
    public int    Threshold      { get; init; }
    public bool   IsComposite    { get; init; }
    public int    ComponentCount { get; init; }
    public bool   IsLowStock => Stock <= Threshold;

    public static ProductViewModel From(IProduct p) => new()
    {
        Id             = p.Id,
        Name           = p.Name,
        Category       = CategoryFromId(p.Id),
        Price          = p.Price,
        WeightKg       = p.WeightKg,
        Stock          = p.Stock,
        Threshold      = p.Threshold,
        IsComposite    = p is CompositeProduct,
        ComponentCount = p is CompositeProduct cp ? cp.GetComponents().Count : 0
    };

    public static string CategoryFromId(string id) => id.Split('-')[0] switch
    {
        "CPU"  => "İşlemci",
        "GPU"  => "Ekran Kartı",
        "RAM"  => "Bellek",
        "SSD"  => "Depolama",
        "MB"   => "Anakart",
        "PSU"  => "Güç Kaynağı",
        "CASE" => "Kasa",
        "COOL" => "Soğutucu",
        "PC"   => "Montaj PC",
        _      => "Diğer"
    };
}

public class CreateProductInput
{
    public string Id        { get; set; } = "";
    public string Name      { get; set; } = "";
    public double Price     { get; set; }
    public double WeightKg  { get; set; } = 0.5;
    public int    Stock     { get; set; }
    public int    Threshold { get; set; }
}
