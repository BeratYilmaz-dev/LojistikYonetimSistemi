namespace LogistikYonetimSistemi.Patterns.Payment;

public class CreditCardPayment : IPaymentStrategy
{
    public string MethodName => "Kredi Kartı";

    public bool Pay(double amount)
    {
        Console.WriteLine($"  [Kredi Kartı] {amount:C} ödendi.");
        return true;
    }
}
