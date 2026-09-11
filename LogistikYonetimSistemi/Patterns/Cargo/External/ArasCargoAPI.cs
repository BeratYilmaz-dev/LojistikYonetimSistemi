namespace LogistikYonetimSistemi.Patterns.Cargo.External;

/// <summary>
/// Aras Kargo'nun gerçek (dış) API'sini simüle eder.
/// Farklı metot imzaları nedeniyle doğrudan kullanılamaz; Adapter gerektirir.
/// </summary>
public class ArasCargoAPI
{
    public string CreateShipment(Dictionary<string, object> data)
    {
        string code = Guid.NewGuid().ToString("N")[..6].ToUpper();
        Console.WriteLine($"  [ArasCargoAPI] Gönderi oluşturuldu: {code}");
        return code;
    }

    public double GetPrice(double weightKg, double distanceKm)
    {
        return (weightKg * 3.5) + (distanceKm * 0.08);
    }
}
