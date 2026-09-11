namespace LogistikYonetimSistemi.Patterns.Cargo.External;

/// <summary>
/// Yurtiçi Kargo'nun gerçek (dış) API'sini simüle eder.
/// </summary>
public class YurticiAPI
{
    public string NewDelivery(Dictionary<string, object> parameters)
    {
        string code = "YK-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
        Console.WriteLine($"  [YurticiAPI] Teslimat oluşturuldu: {code}");
        return code;
    }

    public double ComputeCost(double kg, double distanceKm)
    {
        return (kg * 4.0) + (distanceKm * 0.07);
    }
}
