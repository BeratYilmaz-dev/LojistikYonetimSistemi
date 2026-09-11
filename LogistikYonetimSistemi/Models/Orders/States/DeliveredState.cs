using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Models.Orders.States;

public class DeliveredState : IOrderState
{
    public string StateName => "Teslim Edildi";

    public void StartReturn(Order order)
    {
        order.SetState(new ReturnState());
        Logger.Instance.LogStateChange(order.Id, StateName, "İade");
    }

    public void Approve(Order order)  => throw new InvalidOperationException($"'{StateName}' durumunda 'Onayla' işlemi yapılamaz.");
    public void Prepare(Order order)  => throw new InvalidOperationException($"'{StateName}' durumunda 'Hazırla' işlemi yapılamaz.");
    public void ShipOut(Order order)  => throw new InvalidOperationException($"'{StateName}' durumunda 'Kargoya Ver' işlemi yapılamaz.");
    public void Deliver(Order order)  => throw new InvalidOperationException($"'{StateName}' durumunda 'Teslim Et' işlemi yapılamaz.");
    public void Cancel(Order order)   => throw new InvalidOperationException($"'{StateName}' durumunda sipariş iptal edilemez.");
}
