using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IBillRepository
{
    Task<Bill?> GetByIdAsync(Guid id);
    Task<IEnumerable<Bill>> GetByUserIdAsync(Guid userId, int page, int pageSize);
    Task<IEnumerable<Bill>> GetAllAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<int> CountByUserIdAsync(Guid userId);
    Task<bool> ExistsDuplicateAsync(Guid userId, string merchantName, decimal amount, DateTime date);
    Task<IEnumerable<Bill>> GetApprovedBillsInRangeAsync(DateTime start, DateTime end);
    Task AddAsync(Bill bill);
    Task UpdateAsync(Bill bill);
}
