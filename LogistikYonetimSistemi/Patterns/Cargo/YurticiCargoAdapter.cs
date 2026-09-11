using LogistikYonetimSistemi.Patterns.Cargo.External;

namespace LogistikYonetimSistemi.Patterns.Cargo;

public class YurticiCargoAdapter : ICargoProvider
{
    private readonly YurticiAPI _api;

    public YurticiCargoAdapter(YurticiAPI api) => _api = api;

    public string GenerateTrackingNumber()
        => _api.NewDelivery(new Dictionary<string, object>());

    public double CalculateFee(double weight, double distance)
        => _api.ComputeCost(weight, distance);

    public bool SendPackage(string orderId)
    {
        _api.NewDelivery(new Dictionary<string, object> { { "orderId", orderId } });
        return true;
    }
}
