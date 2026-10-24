using System.Text.Json;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Services;

public class CartService(IHttpContextAccessor accessor)
{
    private const string CartKey = "FOOD_CART";
    private ISession Session => accessor.HttpContext!.Session;

    public List<CartItem> GetCart()
    {
        var json = Session.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json) ? new() :
            JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
    }

    public void Save(List<CartItem> cart) =>
        Session.SetString(CartKey, JsonSerializer.Serialize(cart));

    public void Clear() => Session.Remove(CartKey);
}
