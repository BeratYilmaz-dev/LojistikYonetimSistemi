namespace LogistikYonetimSistemi.Models.Orders.States;

public class CancelledState : IOrderState
{
    public string StateName => "İptal";

    public void Approve(Order order)     => throw new InvalidOperationException($"'{StateName}' son durumdur, işlem yapılamaz.");
    public void Prepare(Order order)     => throw new InvalidOperationException($"'{StateName}' son durumdur, işlem yapılamaz.");
    public void ShipOut(Order order)     => throw new InvalidOperationException($"'{StateName}' son durumdur, işlem yapılamaz.");
    public void Deliver(Order order)     => throw new InvalidOperationException($"'{StateName}' son durumdur, işlem yapılamaz.");
    public void StartReturn(Order order) => throw new InvalidOperationException($"'{StateName}' son durumdur, işlem yapılamaz.");
    public void Cancel(Order order)      => throw new InvalidOperationException($"Sipariş zaten iptal edilmiş.");
}
