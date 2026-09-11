using LogistikYonetimSistemi.Models.Products;

namespace LogistikYonetimSistemi.Models.Orders;

public class OrderItem
{
    public IProduct Product   { get; }
    public int      Quantity  { get; }
    public double   UnitPrice { get; }
    public double   Subtotal  => UnitPrice * Quantity;

    public OrderItem(IProduct product, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Miktar 0'dan büyük olmalıdır.");
        Product   = product;
        Quantity  = quantity;
        UnitPrice = product.Price;
    }
}
