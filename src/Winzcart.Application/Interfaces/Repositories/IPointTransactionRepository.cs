using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IPointTransactionRepository
{
    Task<IEnumerable<PointTransaction>> GetByUserIdAsync(Guid userId, int page, int pageSize);
    Task<int> CountByUserIdAsync(Guid userId);
    Task AddAsync(PointTransaction transaction);
}
