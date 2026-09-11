using LogistikYonetimSistemi.Utils;
using LogistikYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var allOrders = App.OrderFacade.GetAllOrders();

        if (User.IsInRole("Customer"))
        {
            var myOrders = allOrders
                .Where(o => o.Customer.Username == User.Identity!.Name)
                .ToList();

            ViewBag.MyOrderCount = myOrders.Count;
            ViewBag.ActiveOrders = myOrders.Count(o =>
                o.StateName is "Beklemede" or "Onaylandı" or "Hazırlanıyor" or "Kargoda");
            ViewBag.RecentOrders = myOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(8)
                .Select(OrderViewModel.From)
                .ToList();
            ViewBag.IsCustomer = true;
            ViewBag.IsCourier  = false;
            return View();
        }

        if (User.IsInRole("Courier"))
        {
            var kargoOrders = allOrders
                .Where(o => o.StateName == "Kargoda")
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            ViewBag.ActiveDeliveries = kargoOrders.Count;
            ViewBag.DeliveredToday   = allOrders.Count(o =>
                o.StateName == "Teslim Edildi" &&
                o.CreatedAt.Date == DateTime.Today);
            ViewBag.TotalDelivered   = allOrders.Count(o => o.StateName == "Teslim Edildi");
            ViewBag.RecentOrders     = kargoOrders
                .Take(10)
                .Select(OrderViewModel.From)
                .ToList();
            ViewBag.IsCustomer = false;
            ViewBag.IsCourier  = true;
            return View();
        }

        // Admin / Personel — tam istatistik
        var products = App.StockManager.GetAllProducts().Values.ToList();

        ViewBag.TotalProducts = products.Count;
        ViewBag.LowStockCount = products.Count(p => p.Stock <= p.Threshold);
        ViewBag.TotalOrders   = allOrders.Count;
        ViewBag.ActiveOrders  = allOrders.Count(o =>
            o.StateName is "Beklemede" or "Onaylandı" or "Hazırlanıyor" or "Kargoda");
        ViewBag.TotalRevenue  = allOrders
            .Where(o => o.StateName is "Teslim Edildi" or "Kargoda" or "Hazırlanıyor" or "Onaylandı")
            .Sum(o => o.TotalAmount);
        ViewBag.LowStockItems = products
            .Where(p => p.Stock <= p.Threshold)
            .OrderBy(p => p.Stock)
            .Select(ProductViewModel.From)
            .ToList();
        ViewBag.RecentOrders  = allOrders
            .OrderByDescending(o => o.CreatedAt)
            .Take(8)
            .Select(OrderViewModel.From)
            .ToList();
        ViewBag.IsCustomer = false;
        ViewBag.IsCourier  = false;
        return View();
    }
}
