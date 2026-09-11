namespace LogistikYonetimSistemi.Patterns.Fee;

public class BaseCargoFee : IFeeCalculator
{
    private const double RatePerKg = 3.0;
    private const double RatePerKm = 0.05;

    public double Calculate(double weightKg, double distanceKm)
        => (weightKg * RatePerKg) + (distanceKm * RatePerKm);
}
