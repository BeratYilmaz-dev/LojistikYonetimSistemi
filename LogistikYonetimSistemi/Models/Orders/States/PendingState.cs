using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Models.Orders.States;

public class PendingState : IOrderState
{
    public string StateName => "Beklemede";

    public void Approve(Order order)
    {
        order.SetState(new ApprovedState());
        Logger.Instance.LogStateChange(order.Id, StateName, "Onaylandı");
    }

    public void Cancel(Order order)
    {
        order.SetState(new CancelledState());
        Logger.Instance.LogStateChange(order.Id, StateName, "İptal");
    }

    public void Prepare(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Hazırla' işlemi yapılamaz.");
    public void ShipOut(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Kargoya Ver' işlemi yapılamaz.");
    public void Deliver(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Teslim Et' işlemi yapılamaz.");
    public void StartReturn(Order order) => throw new InvalidOperationException($"'{StateName}' durumunda 'İade Başlat' işlemi yapılamaz.");
}
