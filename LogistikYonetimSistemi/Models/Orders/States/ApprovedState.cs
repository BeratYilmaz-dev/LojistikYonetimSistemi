using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Models.Orders.States;

public class ApprovedState : IOrderState
{
    public string StateName => "Onaylandı";

    public void Prepare(Order order)
    {
        order.SetState(new PreparingState());
        Logger.Instance.LogStateChange(order.Id, StateName, "Hazırlanıyor");
    }

    public void Cancel(Order order)
    {
        order.SetState(new CancelledState());
        Logger.Instance.LogStateChange(order.Id, StateName, "İptal");
    }

    public void Approve(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Onayla' işlemi yapılamaz.");
    public void ShipOut(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Kargoya Ver' işlemi yapılamaz.");
    public void Deliver(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Teslim Et' işlemi yapılamaz.");
    public void StartReturn(Order order) => throw new InvalidOperationException($"'{StateName}' durumunda 'İade Başlat' işlemi yapılamaz.");
}
