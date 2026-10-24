using FoodOrderingSystem.Data;
using FoodOrderingSystem.Models;
using FoodOrderingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingSystem.Controllers;

public class CheckoutController(AppDbContext db, CartService cart, RazorpayService razorpay) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (cart.GetCart().Count == 0) return RedirectToAction("Index", "Cart");
        return View(new CheckoutViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var items = cart.GetCart();
        if (items.Count == 0) return RedirectToAction("Index", "Cart");
        if (model.PaymentMethod is not ("COD" or "Razorpay"))
            ModelState.AddModelError(nameof(model.PaymentMethod), "Select a valid payment method.");
        if (!ModelState.IsValid) return View(model);

        // Reload prices from the database so client-side values cannot alter the total.
        var ids = items.Select(x => x.MenuItemId).ToList();
        var menu = await db.MenuItems.Where(x => ids.Contains(x.Id) && x.IsAvailable).ToDictionaryAsync(x => x.Id);
        if (menu.Count != items.Count)
        {
            ModelState.AddModelError("", "One or more items are no longer available. Please review your cart.");
            return View(model);
        }

        var total = items.Sum(x => menu[x.MenuItemId].Price * x.Quantity);
        var order = new CustomerOrder
        {
            CustomerName=model.CustomerName.Trim(), Email=model.Email.Trim(), Address=model.Address.Trim(),
            Phone=model.Phone.Trim(), TotalAmount=total,
            PaymentMethod=model.PaymentMethod,
            Status=model.PaymentMethod == "COD" ? "Placed - Cash on Delivery" : "Pending Payment",
            Items=items.Select(x => new OrderItem { MenuItemId=x.MenuItemId, ItemName=menu[x.MenuItemId].Name,
                UnitPrice=menu[x.MenuItemId].Price, Quantity=x.Quantity }).ToList()
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        if (model.PaymentMethod == "COD")
        {
            cart.Clear();
            return RedirectToAction(nameof(Confirmation), new { id=order.Id });
        }

        try
        {
            var (razorpayOrderId, keyId) = await razorpay.CreateOrderAsync(total, $"food-order-{order.Id}");
            order.RazorpayOrderId = razorpayOrderId;
            await db.SaveChangesAsync();
            ViewBag.RazorpayOrderId = razorpayOrderId;
            ViewBag.RazorpayKeyId = keyId;
            ViewBag.AmountPaise = (long)Math.Round(total * 100m, MidpointRounding.AwayFromZero);
            ViewBag.CustomerName = order.CustomerName;
            ViewBag.Email = order.Email;
            ViewBag.Phone = order.Phone;
            ViewBag.LocalOrderId = order.Id;
            return View("PayOnline");
        }
        catch (Exception)
        {
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            ModelState.AddModelError("", "Online payment setup failed. Verify Razorpay Test Mode keys in appsettings.json, then try again or select Cash on Delivery.");
            return View("Index", model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(int localOrderId, string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
    {
        var order = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == localOrderId);
        if (order is null || string.IsNullOrWhiteSpace(order.RazorpayOrderId) ||
            order.RazorpayOrderId != razorpay_order_id ||
            !razorpay.VerifySignature(razorpay_order_id, razorpay_payment_id, razorpay_signature))
            return BadRequest("Payment verification failed. Do not treat this order as paid.");

        order.RazorpayPaymentId = razorpay_payment_id;
        order.Status = "Paid";
        await db.SaveChangesAsync();
        cart.Clear();
        return RedirectToAction(nameof(Confirmation), new { id=order.Id });
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        if (order is null) return NotFound();
        return View(order);
    }
}
