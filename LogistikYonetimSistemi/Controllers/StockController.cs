using LogistikYonetimSistemi.Utils;
using LogistikYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class StockController : Controller
{
    public IActionResult Index()
    {
        var vm = App.StockManager.GetAllProducts().Values
            .Select(ProductViewModel.From)
            .OrderBy(p => p.IsLowStock ? 0 : 1)
            .ThenBy(p => p.Category)
            .ToList();
        return View(vm);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public IActionResult Update(string productId, int delta, string reason)
    {
        try
        {
            App.StockManager.UpdateStock(productId, delta);
            Logger.Instance.Log($"Stok güncelleme — Ürün: {productId}, Delta: {delta:+#;-#}, Neden: {reason}");
            TempData["Success"] = "Stok güncellendi.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
