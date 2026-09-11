namespace LogistikYonetimSistemi.Patterns.Fee;

public class InsuranceDecorator : FeeDecorator
{
    private const double InsuranceRate = 0.05; // %5

    public InsuranceDecorator(IFeeCalculator wrapped) : base(wrapped) { }

    public override double Calculate(double weightKg, double distanceKm)
    {
        double baseAmount = _wrapped.Calculate(weightKg, distanceKm);
        double insurance  = baseAmount * InsuranceRate;
        Console.WriteLine($"  [Sigorta Ek Ücreti] +{insurance:C}");
        return baseAmount + insurance;
    }
}
