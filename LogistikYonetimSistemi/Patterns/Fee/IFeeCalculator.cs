namespace LogistikYonetimSistemi.Patterns.Fee;

public interface IFeeCalculator
{
    double Calculate(double weightKg, double distanceKm);
}
