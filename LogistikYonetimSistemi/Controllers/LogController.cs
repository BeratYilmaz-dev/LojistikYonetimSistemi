using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize(Roles = "Admin")]
public class LogController : Controller
{
    public IActionResult Index()
    {
        var logPath = LogistikYonetimSistemi.Utils.Logger.Instance.LogFilePath;
        var lines   = System.IO.File.Exists(logPath)
            ? System.IO.File.ReadAllLines(logPath).Reverse().Take(200).ToList()
            : new List<string>();
        ViewBag.LogPath = logPath;
        return View(lines);
    }

    [HttpPost]
    public IActionResult Clear()
    {
        var logPath = LogistikYonetimSistemi.Utils.Logger.Instance.LogFilePath;
        if (System.IO.File.Exists(logPath)) System.IO.File.WriteAllText(logPath, "");
        TempData["Success"] = "Log temizlendi.";
        return RedirectToAction(nameof(Index));
    }
}
