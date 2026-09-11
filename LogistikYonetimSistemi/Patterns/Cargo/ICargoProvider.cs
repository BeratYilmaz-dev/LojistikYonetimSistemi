using LogistikYonetimSistemi.Models.Orders;

namespace LogistikYonetimSistemi.Patterns.Cargo;

public interface ICargoProvider
{
    string GenerateTrackingNumber();
    double CalculateFee(double weight, double distance);
    bool   SendPackage(string orderId);
}
