using Winzcart.Application.Interfaces.Services;

namespace Winzcart.Infrastructure.Services;

public class MockImageStorageService : IImageStorageService
{
    public async Task<string> UploadAsync(Stream imageStream, string fileName, string contentType)
    {
        // Simulate network delay
        await Task.Delay(500);
        
        // Return a fake mock image CDN URL since this is mocked
        string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"https://cdn.winzcart.mock/images/{uniqueId}_{fileName}";
    }
}
