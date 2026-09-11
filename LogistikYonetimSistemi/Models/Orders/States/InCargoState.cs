using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Models.Orders.States;

public class InCargoState : IOrderState
{
    public string StateName => "Kargoda";

    public void Deliver(Order order)
    {
        order.SetState(new DeliveredState());
        Logger.Instance.LogStateChange(order.Id, StateName, "Teslim Edildi");
    }

    public void StartReturn(Order order)
    {
        order.SetState(new ReturnState());
        Logger.Instance.LogStateChange(order.Id, StateName, "İade");
    }

    // Kargodaki sipariş iptal edilemez — iş kuralı burada zorlanıyor
    public void Cancel(Order order)      => throw new InvalidOperationException("Kargodaki sipariş iptal edilemez. Yalnızca iade başlatılabilir.");

    public void Approve(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Onayla' işlemi yapılamaz.");
    public void Prepare(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Hazırla' işlemi yapılamaz.");
    public void ShipOut(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Kargoya Ver' işlemi yapılamaz.");
}
