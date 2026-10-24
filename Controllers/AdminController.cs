using FoodOrderingSystem.Data;
using FoodOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingSystem.Controllers;

public class AdminController(AppDbContext db, IConfiguration config) : Controller
{
    private bool IsAdmin => HttpContext.Session.GetString("IS_ADMIN") == "yes";

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Login(string accessCode)
    {
        var expected = config["Admin:AccessCode"] ?? "FOOD2026";
        if (!string.IsNullOrWhiteSpace(accessCode) && accessCode == expected)
        {
            HttpContext.Session.SetString("IS_ADMIN", "yes");
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Error = "Incorrect admin access code.";
        return View();
    }

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        ViewBag.ItemCount = await db.MenuItems.CountAsync();
        ViewBag.OrderCount = await db.Orders.CountAsync();
        ViewBag.Revenue = await db.Orders.Where(x => x.Status == "Paid" || x.Status == "Placed - Cash on Delivery").SumAsync(x => (decimal?)x.TotalAmount) ?? 0;
        return View(await db.Orders.Include(x => x.Items).OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync());
    }

    public async Task<IActionResult> Menu()
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        return View(await db.MenuItems.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        return View("Edit", new MenuItem { IsAvailable=true, ImageUrl="https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=900&q=80" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(MenuItem item)
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        if (!ModelState.IsValid) return View("Edit", item);
        if (item.Id == 0) db.MenuItems.Add(item);
        else db.MenuItems.Update(item);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Menu));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        var item = await db.MenuItems.FindAsync(id);
        if (item is not null) { item.IsAvailable = !item.IsAvailable; await db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Menu));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("IS_ADMIN");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (!IsAdmin) return RedirectToAction(nameof(Login));
        var allowed = new[] { "Preparing", "Out for delivery", "Delivered", "Cancelled" };
        if (!allowed.Contains(status)) return BadRequest("Invalid order status.");
        var order = await db.Orders.FindAsync(id);
        if (order is not null) { order.Status = status; await db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
