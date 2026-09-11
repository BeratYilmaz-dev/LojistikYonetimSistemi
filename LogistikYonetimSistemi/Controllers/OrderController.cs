using LogistikYonetimSistemi.Models.Orders;
using LogistikYonetimSistemi.Utils;
using LogistikYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogistikYonetimSistemi.Controllers;

[Authorize]
public class OrderController : Controller
{
    private static readonly Dictionary<string, Action<Order>> _advanceActions = new()
    {
        ["prepare"] = o => o.Prepare(),
        ["ship"]    = o => o.ShipOut(),
        ["deliver"] = o => o.Deliver(),
    };

    public IActionResult Index()
    {
        var allOrders = App.OrderFacade.GetAllOrders();

        var filtered = OrderPermissions.CanSeeAllOrders(User)
            ? allOrders
            : allOrders.Where(o => o.Customer.Username == User.Identity!.Name);

        var vm = filtered.OrderByDescending(o => o.CreatedAt)
                         .Select(OrderViewModel.From)
                         .ToList();
        return View(vm);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public IActionResult Create(CreateOrderInput input)
    {
        var customer = App.UserService.FindByUsername(User.Identity!.Name!);
        if (customer == null) return Unauthorized();

        if (input.Items == null || !input.Items.Any(i => i.Quantity > 0))
        {
            TempData["Error"] = "En az bir ürün seçmelisiniz.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var items = input.Items
                .Where(i => i.Quantity > 0)
                .Select(i =>
                {
                    var product = App.StockManager.GetProduct(i.ProductId)
                        ?? throw new KeyNotFoundException($"Ürün bulunamadı: {i.ProductId}");
                    return new OrderItem(product, i.Quantity);
                }).ToList();

            var order = App.OrderFacade.PlaceOrder(
                customer, items, input.PaymentType, input.CargoType,
                input.AddInsurance, input.AddFragileProtection, input.DistanceKm);
            TempData["Success"] = $"Sipariş oluşturuldu: #{order.Id} — Onay bekleniyor.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public IActionResult Approve(string orderId)
    {
        if (!OrderPermissions.CanApprove(User)) return Forbid();

        try
        {
            App.OrderFacade.ApproveOrder(orderId);
            TempData["Success"] = "Sipariş onaylandı.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Staff,Courier")]
    [HttpPost]
    public IActionResult Advance(string orderId, string action)
    {
        if (!OrderPermissions.CanAdvance(User))                          return Forbid();
        if (action == "deliver" && !OrderPermissions.CanDeliver(User))   return Forbid();

        if (!_advanceActions.TryGetValue(action, out var advance))
        {
            TempData["Error"] = "Bilinmeyen sipariş işlemi.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var order = App.OrderFacade.FindOrder(orderId);
            advance(order);
            TempData["Success"] = $"Sipariş durumu: {order.StateName}";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Cancel(string orderId)
    {
        try
        {
            App.OrderFacade.CancelOrder(
                orderId,
                callerUsername: User.Identity!.Name!,
                callerRole:     User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "");
            TempData["Success"] = "Sipariş iptal edildi.";
        }
        catch (UnauthorizedAccessException ex) { TempData["Error"] = ex.Message; }
        catch (Exception ex)                   { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Return(string orderId)
    {
        try
        {
            App.OrderFacade.InitiateReturn(
                orderId,
                callerUsername: User.Identity!.Name!,
                callerRole:     User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "");
            TempData["Success"] = "İade başlatıldı. Stoklar geri yüklendi.";
        }
        catch (UnauthorizedAccessException ex) { TempData["Error"] = ex.Message; }
        catch (Exception ex)                   { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
