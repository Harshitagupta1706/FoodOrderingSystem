using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystem.Models;

public class CustomerOrder
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Required, StringLength(300)]
    public string Address { get; set; } = "";

    [Required, Phone, StringLength(20)]
    public string Phone { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    [Required]
    public string Status { get; set; } = "Pending Payment";

    [Required]
    public string PaymentMethod { get; set; } = "COD";

    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
