using FoodOrderingSystem.Data;
using FoodOrderingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingSystem.Controllers;

public class CartController(AppDbContext db, CartService cartService) : Controller
{
    public IActionResult Index() => View(cartService.GetCart());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id)
    {
        var item = await db.MenuItems.FirstOrDefaultAsync(x => x.Id == id && x.IsAvailable);
        if (item is null) return NotFound();
        var cart = cartService.GetCart();
        var existing = cart.FirstOrDefault(x => x.MenuItemId == id);
        if (existing is null)
            cart.Add(new() { MenuItemId=item.Id, Name=item.Name, Price=item.Price, ImageUrl=item.ImageUrl, Quantity=1 });
        else if (existing.Quantity < 20) existing.Quantity++;
        cartService.Save(cart);
        TempData["Message"] = $"{item.Name} added to your cart.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(int id, int quantity)
    {
        var cart = cartService.GetCart();
        var item = cart.FirstOrDefault(x => x.MenuItemId == id);
        if (item is not null)
        {
            if (quantity <= 0) cart.Remove(item);
            else item.Quantity = Math.Clamp(quantity, 1, 20);
        }
        cartService.Save(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        cartService.Clear();
        return RedirectToAction(nameof(Index));
    }
}
