using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<IEnumerable<AppUser>> GetAllAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task AddAsync(AppUser user);
    Task UpdateAsync(AppUser user);
}
