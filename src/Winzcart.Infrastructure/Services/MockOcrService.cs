using Winzcart.Application.Interfaces.Services;

namespace Winzcart.Infrastructure.Services;

public class MockOcrService : IOcrService
{
    private readonly Random _random = new Random();
    private readonly string[] _merchantNames = { "SuperMart", "TechStore", "Cafe Delight", "Book Haven", "Fresh Grocers" };

    public async Task<OcrResult> ExtractAsync(string imageUrl)
    {
        // Simulate network delay
        await Task.Delay(1000);

        // Simulate 90% success rate
        if (_random.NextDouble() > 0.9)
        {
            return new OcrResult
            {
                Success = false,
                ErrorMessage = "OCR processing failed or image was unclear."
            };
        }

        // Generate random mock data
        return new OcrResult
        {
            Success = true,
            MerchantName = _merchantNames[_random.Next(_merchantNames.Length)],
            TotalAmount = (decimal)(_random.Next(50, 5000) + _random.NextDouble()),
            BillDate = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
        };
    }
}
