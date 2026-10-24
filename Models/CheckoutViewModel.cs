using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystem.Models;

public class CheckoutViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string CustomerName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(300)]
    [Display(Name = "Delivery address")]
    public string Address { get; set; } = "";

    [Required, Phone, StringLength(20)]
    public string Phone { get; set; } = "";

    [Required]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "COD";
}
