namespace LogistikYonetimSistemi.Models.Orders.States;

public interface IOrderState
{
    string StateName { get; }
    void Approve(Order order);
    void Prepare(Order order);
    void ShipOut(Order order);
    void Deliver(Order order);
    void StartReturn(Order order);
    void Cancel(Order order);
}
