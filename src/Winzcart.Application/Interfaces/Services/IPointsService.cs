namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Points;
using Winzcart.Domain.Enums;

public interface IPointsService
{
    Task<PointsBalanceResponse> GetBalanceAsync(Guid userId);
    Task<IEnumerable<PointTransactionResponse>> GetHistoryAsync(Guid userId, int page, int pageSize);
    Task AddTransactionAsync(Guid userId, int points, PointTransactionType type, string description);
}
