using AppLogLevel = LogistikYonetimSistemi.Models.Enums.LogLevel;

namespace LogistikYonetimSistemi.Utils;

// Singleton — static constructor ile CLR garantili thread-safe başlatma
public sealed class Logger
{
    private static readonly Logger _instance;
    private readonly string        _logFilePath;
    private readonly object        _writeLock = new();

    static Logger()
    {
        _instance = new Logger();
    }

    private Logger()
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDir);
        _logFilePath = Path.Combine(logDir, "sistem_log.txt");
    }

    public static Logger Instance => _instance;
    public string LogFilePath     => _logFilePath;

    public void Log(string message, AppLogLevel level = AppLogLevel.Info)
    {
        string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        Console.WriteLine(entry);
        lock (_writeLock)
        {
            File.AppendAllText(_logFilePath, entry + Environment.NewLine);
        }
    }

    public void LogStockChange(string productId, int fromStock, int toStock)
        => Log($"STOK DEĞİŞİMİ — Ürün: {productId}, Eski: {fromStock}, Yeni: {toStock}");

    public void LogPayment(string orderId, string method, double amount)
        => Log($"ÖDEME — Sipariş: {orderId}, Yöntem: {method}, Tutar: {amount:C}");

    public void LogStateChange(string orderId, string fromState, string toState)
        => Log($"DURUM GEÇİŞİ — Sipariş: {orderId}, {fromState} → {toState}");
}
