namespace LogistikYonetimSistemi.Patterns.Payment;

public interface IPaymentStrategy
{
    string MethodName { get; }
    bool Pay(double amount);
}
