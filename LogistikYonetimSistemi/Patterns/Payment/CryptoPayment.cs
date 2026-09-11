namespace LogistikYonetimSistemi.Patterns.Payment;

public class CryptoPayment : IPaymentStrategy
{
    public string MethodName => "Kripto";

    public bool Pay(double amount)
    {
        Console.WriteLine($"  [Kripto] {amount:C} ödendi.");
        return true;
    }
}
