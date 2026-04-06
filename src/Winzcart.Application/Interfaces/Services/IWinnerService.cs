namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Admin;

public interface IWinnerService
{
    Task<WeeklyWinnerResponse> SelectWinnerAsync(SelectWinnerRequest request);
    Task<IEnumerable<WeeklyWinnerResponse>> GetWinnersAsync(int page, int pageSize);
}
