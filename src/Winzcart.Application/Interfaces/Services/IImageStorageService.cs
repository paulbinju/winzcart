namespace Winzcart.Application.Interfaces.Services;

public interface IImageStorageService
{
    Task<string> UploadAsync(Stream imageStream, string fileName, string contentType);
}
