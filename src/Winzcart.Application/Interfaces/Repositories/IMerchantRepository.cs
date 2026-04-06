using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IMerchantRepository
{
    Task<Merchant?> GetByIdAsync(Guid id);
    Task<Merchant?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Merchant>> GetAllAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task AddAsync(Merchant merchant);
    Task UpdateAsync(Merchant merchant);
}
