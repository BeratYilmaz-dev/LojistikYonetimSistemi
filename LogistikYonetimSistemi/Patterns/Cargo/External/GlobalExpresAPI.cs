namespace LogistikYonetimSistemi.Patterns.Cargo.External;

/// <summary>
/// GlobalExpres'in gerçek (dış) API'sini simüle eder.
/// </summary>
public class GlobalExpresAPI
{
    public string RegisterPackage(Dictionary<string, object> info)
    {
        string code = "GX-" + DateTime.Now.Ticks.ToString()[^6..];
        Console.WriteLine($"  [GlobalExpresAPI] Paket kaydedildi: {code}");
        return code;
    }

    public double GetPricing(double weightKg, double distanceKm)
    {
        return (weightKg * 5.0) + (distanceKm * 0.12);
    }
}
