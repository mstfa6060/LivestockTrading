using System.Text.Json;

namespace LivestockTrading.Application.Services;

/// <summary>
/// Apple App Store ve Google Play IAP receipt doğrulama servisi
/// </summary>
public class IAPValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IAPValidationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Apple App Store receipt doğrulama
    /// Sandbox ve Production ortamlarını otomatik dener
    /// </summary>
    public async Task<IAPValidationResult> ValidateAppleReceipt(string receipt, CancellationToken ct = default)
    {
        // Önce production dene, 21007 dönerse sandbox'a geç
        var result = await ValidateAppleReceiptInternal(
            "https://buy.itunes.apple.com/verifyReceipt", receipt, ct);

        if (result.StatusCode == 21007)
        {
            result = await ValidateAppleReceiptInternal(
                "https://sandbox.itunes.apple.com/verifyReceipt", receipt, ct);
        }

        return result;
    }

    private async Task<IAPValidationResult> ValidateAppleReceiptInternal(
        string url, string receipt, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var payload = JsonSerializer.Serialize(new { receipt_data = receipt });
            var response = await client.PostAsync(url,
                new StringContent(payload, System.Text.Encoding.UTF8, "application/json"), ct);

            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                return new IAPValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Apple API error: {response.StatusCode}",
                    RawResponse = content
                };
            }

            var json = JsonDocument.Parse(content);
            var status = json.RootElement.GetProperty("status").GetInt32();

            return new IAPValidationResult
            {
                IsValid = status == 0,
                StatusCode = status,
                TransactionId = TryGetString(json.RootElement, "latest_receipt_info", "transaction_id"),
                ProductId = TryGetString(json.RootElement, "latest_receipt_info", "product_id"),
                RawResponse = content,
                ErrorMessage = status != 0 ? $"Apple validation failed with status: {status}" : null
            };
        }
        catch (Exception ex)
        {
            return new IAPValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Apple validation exception: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Google Play receipt doğrulama
    /// </summary>
    public async Task<IAPValidationResult> ValidateGoogleReceipt(
        string purchaseToken, string productId, string packageName, CancellationToken ct = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://androidpublisher.googleapis.com/androidpublisher/v3/applications/{packageName}/purchases/subscriptions/{productId}/tokens/{purchaseToken}";

            var response = await client.GetAsync(url, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                return new IAPValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Google API error: {response.StatusCode}",
                    RawResponse = content
                };
            }

            var json = JsonDocument.Parse(content);
            var paymentState = json.RootElement.TryGetProperty("paymentState", out var ps)
                ? ps.GetInt32() : -1;

            return new IAPValidationResult
            {
                IsValid = paymentState == 1, // 1 = received
                TransactionId = json.RootElement.TryGetProperty("orderId", out var oid) ? oid.GetString() : null,
                ProductId = productId,
                RawResponse = content,
                ErrorMessage = paymentState != 1 ? $"Google payment state: {paymentState}" : null
            };
        }
        catch (Exception ex)
        {
            return new IAPValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Google validation exception: {ex.Message}"
            };
        }
    }

    private static string TryGetString(JsonElement root, string arrayProp, string valueProp)
    {
        if (root.TryGetProperty(arrayProp, out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in arr.EnumerateArray())
            {
                if (item.TryGetProperty(valueProp, out var val))
                    return val.GetString();
            }
        }
        return null;
    }
}

public class IAPValidationResult
{
    public bool IsValid { get; set; }
    public int StatusCode { get; set; }
    public string TransactionId { get; set; }
    public string ProductId { get; set; }
    public string ErrorMessage { get; set; }
    public string RawResponse { get; set; }
}
