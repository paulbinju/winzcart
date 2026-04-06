using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IWeeklyWinnerRepository
{
    Task<WeeklyWinner?> GetByIdAsync(Guid id);
    Task<IEnumerable<WeeklyWinner>> GetAllAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<bool> ExistsForWeekAsync(DateTime weekStart, DateTime weekEnd);
    Task AddAsync(WeeklyWinner winner);
}
