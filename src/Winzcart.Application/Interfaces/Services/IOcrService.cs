namespace Winzcart.Application.Interfaces.Services;

public class OcrResult
{
    public string MerchantName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime BillDate { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IOcrService
{
    Task<OcrResult> ExtractAsync(string imageUrl);
}
