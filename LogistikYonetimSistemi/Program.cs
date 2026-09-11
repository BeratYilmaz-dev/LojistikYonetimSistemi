using LogistikYonetimSistemi.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

 //eski oturum cookie'lerini geçersiz kılmak için
builder.Services.AddDataProtection()
       .SetApplicationName($"LYS-{Guid.NewGuid():N}");

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath        = "/Account/Login";
        opt.AccessDeniedPath = "/Account/Login";
        opt.ExpireTimeSpan   = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Smtp config
App.Initialize();

app.Run();
