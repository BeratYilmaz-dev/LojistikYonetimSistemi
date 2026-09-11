using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Patterns.Notification;
using LogistikYonetimSistemi.Services;

namespace LogistikYonetimSistemi.Utils;

/// <summary>Uygulama genelinde tekil servis örneklerini tutar.</summary>
public static class App
{
    public static StockManager   StockManager { get; } = new();
    public static UserService    UserService  { get; } = new();
    public static AuthService    AuthService  { get; } = new(UserService);

    // Dış dünya Proxy üzerinden erişir; gerçek Facade içeride kalır (Protection Proxy).
    private static readonly OrderFacade _realOrderFacade = new(StockManager);
    public  static IOrderFacade OrderFacade { get; } = new AuthorizedOrderFacade(_realOrderFacade);

    public static void Initialize()
    {
        var smtp = new SmtpSettings();

        StockManager.RegisterObserver(
            new EmailNotificationObserver(
                () => UserService.GetAll()
                                 .OfType<Staff>()
                                 .Where(s => s.Department == "Satın Alma")
                                 .Select(s => s.Email),
                smtp));

        StockManager.RegisterObserver(new SystemNotificationObserver("depo-sorumlusu-001"));

        DataSeeder.Seed();
    }
}
