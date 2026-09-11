namespace LogistikYonetimSistemi.Patterns.Payment;

public class WireTransferPayment : IPaymentStrategy
{
    public string MethodName => "Havale/EFT";

    public bool Pay(double amount)
    {
        Console.WriteLine($"  [Havale/EFT] {amount:C} ödendi.");
        return true;
    }
}
