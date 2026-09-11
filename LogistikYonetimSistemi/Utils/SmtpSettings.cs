namespace LogistikYonetimSistemi.Utils;

/// appsettings.json → "SmtpSettings" bölümüne karşılık gelen POCO
public class SmtpSettings
{
    public bool   Enabled   { get; set; } = true;
    public string Host      { get; set; } = "smtp.gmail.com";
    public int    Port      { get; set; } = 587;
    public bool   EnableSsl { get; set; } = true;
    public string From      { get; set; } = "beratyilmaz1814@gmail.com";
    public string Username  { get; set; } = "beratyilmaz1814@gmail.com";
    public string Password  { get; set; } = "gshphlhodsjscveb";
}
