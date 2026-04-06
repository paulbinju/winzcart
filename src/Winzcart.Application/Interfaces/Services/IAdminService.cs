namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Admin;

public interface IAdminService
{
    Task<AnalyticsResponse> GetAnalyticsAsync();
}
