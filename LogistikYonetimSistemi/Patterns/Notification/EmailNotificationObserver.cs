using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using AppLogLevel = LogistikYonetimSistemi.Models.Enums.LogLevel;

namespace LogistikYonetimSistemi.Patterns.Notification;

/// <summary>
/// Observer: stok eşik altına düştüğünde alıcı listesine e-posta gönderir.
/// Alıcılar çalışma zamanında <paramref name="emailResolver"/> ile sorgulanır.
/// SmtpSettings.Enabled = false ise yalnızca loglanır (demo modu).
/// </summary>
public class EmailNotificationObserver : IStockObserver
{
    private readonly Func<IEnumerable<string>> _emailResolver;
    private readonly SmtpSettings              _smtp;

    public EmailNotificationObserver(Func<IEnumerable<string>> emailResolver, SmtpSettings smtp)
    {
        _emailResolver = emailResolver;
        _smtp          = smtp;
    }

    public void OnStockLow(IProduct product, int currentStock)
    {
        var recipients = _emailResolver().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

        string subject = $"[KRİTİK STOK] {product.Name} — Mevcut: {currentStock}";
        string body    = $"""
            Sayın Satın Alma Birimi,

            "{product.Name}" (ID: {product.Id}) ürününün stoğu kritik eşik değerinin
            altına düşmüştür.

              Mevcut Stok : {currentStock}
              Eşik Değer  : {product.Threshold}
              Tarih/Saat  : {DateTime.Now:dd.MM.yyyy HH:mm}

            Lütfen stok yenilemesi için gerekli işlemi başlatınız.

            Lojistik Yönetim Sistemi — Otomatik Bildirim
            """;

        if (recipients.Count == 0)
        {
            Logger.Instance.Log(
                $"[E-POSTA UYARISI] {subject} — alıcı bulunamadı (Satın Alma personeli yok?)",
                AppLogLevel.Warning);
            return;
        }

        Logger.Instance.Log(
            $"[E-POSTA UYARISI → {string.Join(", ", recipients)}] {subject}",
            AppLogLevel.Warning);

        if (!_smtp.Enabled)
        {
            Logger.Instance.Log(
                "SMTP devre dışı — e-posta simüle edildi (SmtpSettings.Enabled = true yapın)",
                AppLogLevel.Info);
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await SendAsync(subject, body, recipients);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log($"E-posta gönderilemedi: {ex.Message}", AppLogLevel.Error);
            }
        });
    }

    private async Task SendAsync(string subject, string body, List<string> recipients)
    {
        using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(30));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Lojistik YS", _smtp.From));
        foreach (var r in recipients)
            message.To.Add(MailboxAddress.Parse(r));
        message.Subject = subject;
        message.Body    = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls, cts.Token);
        await client.AuthenticateAsync(_smtp.Username, _smtp.Password, cts.Token);
        await client.SendAsync(message, cts.Token);
        await client.DisconnectAsync(true, cts.Token);

        Logger.Instance.Log($"E-posta gönderildi → {string.Join(", ", recipients)}");
    }
}
