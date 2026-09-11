using LogistikYonetimSistemi.Patterns.Cargo.External;

namespace LogistikYonetimSistemi.Patterns.Cargo;

public class GlobalExpresAdapter : ICargoProvider
{
    private readonly GlobalExpresAPI _api;

    public GlobalExpresAdapter(GlobalExpresAPI api) => _api = api;

    public string GenerateTrackingNumber()
        => _api.RegisterPackage(new Dictionary<string, object>());

    public double CalculateFee(double weight, double distance)
        => _api.GetPricing(weight, distance);

    public bool SendPackage(string orderId)
    {
        _api.RegisterPackage(new Dictionary<string, object> { { "orderId", orderId } });
        return true;
    }
}
