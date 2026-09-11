namespace LogistikYonetimSistemi.Patterns.Fee;

public class FragileProtectionDecorator : FeeDecorator
{
    private const double FixedFee = 25.0;

    public FragileProtectionDecorator(IFeeCalculator wrapped) : base(wrapped) { }

    public override double Calculate(double weightKg, double distanceKm)
    {
        double baseAmount = _wrapped.Calculate(weightKg, distanceKm);
        Console.WriteLine($"  [Kırılgan Koruma Ek Ücreti] +{FixedFee:C}");
        return baseAmount + FixedFee;
    }
}
