using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Models.Orders;

namespace LogistikYonetimSistemi.ViewModels;

public class OrderViewModel
{
    public string   Id                   { get; init; } = "";
    public string   CustomerName         { get; init; } = "";
    public string   Status               { get; init; } = "";
    public double   ProductTotal         { get; init; }
    public double   CargoFee             { get; init; }
    public double   TotalAmount          { get; init; }
    public bool     HasInsurance         { get; init; }
    public bool     HasFragileProtection { get; init; }
    public DateTime CreatedAt            { get; init; }
    public string?  TrackingNumber       { get; init; }
    public List<string> ItemSummaries    { get; init; } = new();

    public static OrderViewModel From(Order o) => new()
    {
        Id                   = o.Id,
        CustomerName         = o.Customer.Username,
        Status               = o.StateName,
        ProductTotal         = o.Items.Sum(i => i.Subtotal),
        CargoFee             = o.CargoFee,
        TotalAmount          = o.TotalAmount,
        HasInsurance         = o.HasInsurance,
        HasFragileProtection = o.HasFragileProtection,
        CreatedAt            = o.CreatedAt,
        TrackingNumber       = o.TrackingNumber,
        ItemSummaries        = o.Items.Select(i => $"{i.Product.Name} x{i.Quantity}").ToList()
    };
}

public class CreateOrderInput
{
    public List<OrderItemInput> Items                { get; set; } = new();
    public PaymentType          PaymentType          { get; set; }
    public CargoType            CargoType            { get; set; }
    public bool                 AddInsurance         { get; set; }
    public bool                 AddFragileProtection { get; set; }
    /// <summary>Teslimat mesafesi (km). Kullanıcı seçer; kargo ücretini doğrudan etkiler.</summary>
    public int                  DistanceKm           { get; set; } = 100;
}

public class OrderItemInput
{
    public string ProductId { get; set; } = "";
    public int    Quantity  { get; set; }
}
