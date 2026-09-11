using LogistikYonetimSistemi.Models.Users;
using LogistikYonetimSistemi.Utils;
using LogistikYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    public IActionResult Index()
    {
        var vm = App.UserService.GetAll().Select(UserViewModel.From).ToList();
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(CreateUserInput input)
    {
        if (App.UserService.FindByUsername(input.Username) != null)
        {
            TempData["Error"] = "Bu kullanıcı adı zaten kullanılıyor.";
            return RedirectToAction(nameof(Index));
        }

        var hash    = Models.Users.User.HashPassword(input.Password);
        var newUser = UserFactory.Create(input, hash);

        App.UserService.Add(newUser);
        TempData["Success"] = $"Kullanıcı oluşturuldu: {input.Username}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(string userId)
    {
        var me = App.UserService.FindByUsername(User.Identity!.Name!);
        if (me?.Id == userId)
        {
            TempData["Error"] = "Kendi hesabınızı silemezsiniz.";
            return RedirectToAction(nameof(Index));
        }
        App.UserService.Remove(userId);
        TempData["Success"] = "Kullanıcı silindi.";
        return RedirectToAction(nameof(Index));
    }
}
