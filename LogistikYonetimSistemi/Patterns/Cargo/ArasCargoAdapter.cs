using LogistikYonetimSistemi.Patterns.Cargo.External;

namespace LogistikYonetimSistemi.Patterns.Cargo;

public class ArasCargoAdapter : ICargoProvider
{
    private readonly ArasCargoAPI _api;

    public ArasCargoAdapter(ArasCargoAPI api) => _api = api;

    public string GenerateTrackingNumber()
    {
        string code = _api.CreateShipment(new Dictionary<string, object>());
        return $"ARAS-{code}";
    }

    public double CalculateFee(double weight, double distance)
        => _api.GetPrice(weight, distance);

    public bool SendPackage(string orderId)
    {
        _api.CreateShipment(new Dictionary<string, object> { { "orderId", orderId } });
        return true;
    }
}
