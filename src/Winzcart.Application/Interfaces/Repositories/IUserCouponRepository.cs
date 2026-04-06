using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface IUserCouponRepository
{
    Task<UserCoupon?> GetByIdAsync(Guid id);
    Task<UserCoupon?> GetByCodeAsync(string code);
    Task<IEnumerable<UserCoupon>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserCoupon userCoupon);
    Task UpdateAsync(UserCoupon userCoupon);
}
