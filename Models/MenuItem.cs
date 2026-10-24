using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystem.Models;

public class MenuItem
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = "";

    [Required, StringLength(500)]
    public string Description { get; set; } = "";

    [Required, StringLength(40)]
    public string Category { get; set; } = "Other";

    [Range(1, 100000)]
    public decimal Price { get; set; }

    [Required]
    public string ImageUrl { get; set; } = "";

    public bool IsAvailable { get; set; } = true;
}
