using LogistikYonetimSistemi.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogistikYonetimSistemi.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = App.AuthService.Login(username, password);
        if (user == null)
        {
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,           user.Username),
            new(ClaimTypes.Role,           user.Role.ToString()),
            new("UserId",                  user.Id)
        };

        // Staff ise departman claim olarak eklenir; yetki kontrollerinde kullanılır.
        if (user is Models.Users.Staff staffUser)
            claims.Add(new("Department", staffUser.Department));
        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string username, string password, string confirmPassword,
                                  string email, string address)
    {
        if (password != confirmPassword)
        {
            ViewBag.Error = "Şifreler eşleşmiyor.";
            return View();
        }
        if (App.UserService.FindByUsername(username) != null)
        {
            ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
            return View();
        }
        var customer = new Models.Users.Customer(
            username,
            Models.Users.User.HashPassword(password),
            email,
            address
        );
        App.UserService.Add(customer);
        Utils.Logger.Instance.Log($"Yeni müşteri kaydı: {username}");
        TempData["RegisterSuccess"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
