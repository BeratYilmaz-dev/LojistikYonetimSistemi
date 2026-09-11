using LogistikYonetimSistemi.Utils;

namespace LogistikYonetimSistemi.Models.Orders.States;

public class PreparingState : IOrderState
{
    public string StateName => "Hazırlanıyor";

    public void ShipOut(Order order)
    {
        order.SetState(new InCargoState());
        Logger.Instance.LogStateChange(order.Id, StateName, "Kargoda");
    }

    public void Approve(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Onayla' işlemi yapılamaz.");
    public void Prepare(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Hazırla' işlemi yapılamaz.");
    public void Deliver(Order order)     => throw new InvalidOperationException($"'{StateName}' durumunda 'Teslim Et' işlemi yapılamaz.");
    public void StartReturn(Order order) => throw new InvalidOperationException($"'{StateName}' durumunda 'İade Başlat' işlemi yapılamaz.");
    public void Cancel(Order order)      => throw new InvalidOperationException($"'{StateName}' durumunda sipariş iptal edilemez.");
}
