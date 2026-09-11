namespace LogistikYonetimSistemi.Patterns.Fee;

public abstract class FeeDecorator : IFeeCalculator
{
    protected readonly IFeeCalculator _wrapped;

    protected FeeDecorator(IFeeCalculator wrapped) => _wrapped = wrapped;

    public abstract double Calculate(double weightKg, double distanceKm);
}
