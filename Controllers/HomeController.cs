using FoodOrderingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingSystem.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, string? category)
    {
        var query = db.MenuItems.AsNoTracking().Where(x => x.IsAvailable);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Name.Contains(q) || x.Description.Contains(q));
        if (!string.IsNullOrWhiteSpace(category) && category != "All")
            query = query.Where(x => x.Category == category);

        ViewBag.Query = q ?? "";
        ViewBag.Category = category ?? "All";
        ViewBag.Categories = await db.MenuItems.Where(x => x.IsAvailable)
            .Select(x => x.Category).Distinct().OrderBy(x => x).ToListAsync();
        return View(await query.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync());
    }

    public IActionResult Error() => View();
}
