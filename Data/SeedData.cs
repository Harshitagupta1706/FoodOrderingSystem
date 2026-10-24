using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Data;

public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.MenuItems.Any()) return;

        db.MenuItems.AddRange(
            new MenuItem { Name="Classic Burger", Description="Juicy veg patty, lettuce, tomato and house sauce.", Category="Burgers", Price=149, ImageUrl="https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80" },
            new MenuItem { Name="Margherita Pizza", Description="Classic pizza with tomato, mozzarella and basil.", Category="Pizza", Price=249, ImageUrl="https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=900&q=80" },
            new MenuItem { Name="Crispy French Fries", Description="Golden, crispy fries with a pinch of seasoning.", Category="Sides", Price=99, ImageUrl="https://images.unsplash.com/photo-1573080496219-bb080dd4f877?auto=format&fit=crop&w=900&q=80" },
            new MenuItem { Name="Paneer Tikka Wrap", Description="Paneer tikka, crunchy vegetables and mint sauce.", Category="Wraps", Price=179, ImageUrl="https://images.unsplash.com/photo-1626700051175-6818013e1d4f?auto=format&fit=crop&w=900&q=80" },
            new MenuItem { Name="Chocolate Brownie", Description="Rich chocolate brownie for a sweet finish.", Category="Desserts", Price=119, ImageUrl="https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=900&q=80" },
            new MenuItem { Name="Cold Coffee", Description="Chilled coffee blended until smooth and creamy.", Category="Drinks", Price=109, ImageUrl="https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=900&q=80" }
        );
        db.SaveChanges();
    }
}
