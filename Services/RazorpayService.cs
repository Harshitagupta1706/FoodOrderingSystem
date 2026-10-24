using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FoodOrderingSystem.Services;

public class RazorpayService(HttpClient http, IConfiguration config)
{
    private string KeyId => config["Razorpay:KeyId"] ?? "";
    private string KeySecret => config["Razorpay:KeySecret"] ?? "";
    public bool IsConfigured => !string.IsNullOrWhiteSpace(KeyId) && !string.IsNullOrWhiteSpace(KeySecret);

    public async Task<(string OrderId, string KeyId)> CreateOrderAsync(decimal amount, string receipt)
    {
        if (!IsConfigured) throw new InvalidOperationException("Razorpay test keys are not configured. Follow README.md.");
        var bytes = Encoding.UTF8.GetBytes($"{KeyId}:{KeySecret}");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        var payload = JsonSerializer.Serialize(new
        {
            amount = (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero),
            currency = "INR",
            receipt,
            payment_capture = 1
        });
        using var response = await http.PostAsync("https://api.razorpay.com/v1/orders",
            new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Razorpay order creation failed. Check your Test Mode keys and internet connection.");
        using var doc = JsonDocument.Parse(body);
        return (doc.RootElement.GetProperty("id").GetString()!, KeyId);
    }

    public bool VerifySignature(string orderId, string paymentId, string signature)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(signature)) return false;
        var payload = $"{orderId}|{paymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(KeySecret));
        var hash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hash), Encoding.UTF8.GetBytes(signature.ToLowerInvariant()));
    }
}
