using LogistikYonetimSistemi.Models.Orders.States;
using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Patterns.Cargo;
using LogistikYonetimSistemi.Patterns.Payment;

namespace LogistikYonetimSistemi.Models.Orders;

public class Order
{
    private IOrderState _currentState;

    public string              Id              { get; }
    public User                Customer        { get; }
    public List<OrderItem>     Items           { get; }
    public IPaymentStrategy    Payment         { get; }
    public ICargoProvider      Cargo           { get; }
    public string?             TrackingNumber  { get; set; }
    public DateTime            CreatedAt       { get; }

    public double CargoFee    { get; private set; }
    public bool   HasInsurance       { get; private set; }
    public bool   HasFragileProtection { get; private set; }

    /// Ürün toplamı + kargo ücreti
    public double TotalAmount => Items.Sum(i => i.Subtotal) + CargoFee;
    public string StateName   => _currentState.StateName;

    public void SetCargoFee(double fee, bool insurance, bool fragile)
    {
        CargoFee             = fee;
        HasInsurance         = insurance;
        HasFragileProtection = fragile;
    }

    public Order(User customer, List<OrderItem> items, IPaymentStrategy payment, ICargoProvider cargo)
    {
        Id            = Guid.NewGuid().ToString("N")[..8].ToUpper();
        Customer      = customer;
        Items         = items;
        Payment       = payment;
        Cargo         = cargo;
        CreatedAt     = DateTime.Now;
        _currentState = new PendingState();
    }

    public void Approve()      => _currentState.Approve(this);
    public void Prepare()      => _currentState.Prepare(this);
    public void ShipOut()      => _currentState.ShipOut(this);
    public void Deliver()      => _currentState.Deliver(this);
    public void StartReturn()  => _currentState.StartReturn(this);
    public void Cancel()       => _currentState.Cancel(this);

    public void SetState(IOrderState state) => _currentState = state;

    public override string ToString() =>
        $"Sipariş [{Id}] — Durum: {StateName}, Tutar: {TotalAmount:C}, Müşteri: {Customer.Username}";
}
