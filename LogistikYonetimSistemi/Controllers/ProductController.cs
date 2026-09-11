using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Utils;
using LogistikYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class ProductController : Controller
{
    public IActionResult Index(string? search, string? category)
    {
        var products = App.StockManager.GetAllProducts().Values
            .Select(ProductViewModel.From)
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
            products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(category) && category != "Tümü")
            products = products.Where(p => p.Category == category).ToList();

        ViewBag.Search     = search;
        ViewBag.Category   = category;
        ViewBag.Categories = new[] { "Tümü","İşlemci","Ekran Kartı","Bellek","Depolama","Anakart","Güç Kaynağı","Kasa","Soğutucu","Montaj PC" };

        return View(products);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public IActionResult Create(CreateProductInput input)
    {
        if (App.StockManager.GetProduct(input.Id) != null)
            return BadRequest("Bu ID zaten mevcut.");

        var product = new SimpleProduct(input.Id, input.Name, input.Price, input.Stock, input.Threshold,
                                        weightKg: input.WeightKg > 0 ? input.WeightKg : 0.5);
        App.StockManager.AddProduct(product);
        TempData["Success"] = $"'{input.Name}' ürünü eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public IActionResult Edit(string id, string name, double price, int threshold, double weightKg)
    {
        var product = App.StockManager.GetProduct(id);
        if (product == null) return NotFound();

        if (product is SimpleProduct sp)
        {
            sp.Name      = name;
            sp.Price     = price;
            sp.Threshold = threshold;
            if (weightKg > 0) sp.WeightKg = weightKg;
            Logger.Instance.Log($"Ürün güncellendi: {id} — Ad: {name}, Fiyat: {price:N0} ₺, Eşik: {threshold}");
            TempData["Success"] = $"'{name}' güncellendi.";
        }
        else
        {
            TempData["Error"] = "Montaj ürünleri doğrudan düzenlenemez.";
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Delete(string id)
    {
        var product = App.StockManager.GetProduct(id);
        if (product == null) return NotFound();

        App.StockManager.RemoveProduct(id);
        TempData["Success"] = $"Ürün silindi: {id}";
        return RedirectToAction(nameof(Index));
    }
}
