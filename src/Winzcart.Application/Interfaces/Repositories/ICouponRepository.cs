using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Repositories;

public interface ICouponRepository
{
    Task<Coupon?> GetByIdAsync(Guid id);
    Task<IEnumerable<Coupon>> GetByMerchantIdAsync(Guid merchantId);
    Task<IEnumerable<Coupon>> GetAvailableAsync();
    Task AddAsync(Coupon coupon);
    Task UpdateAsync(Coupon coupon);
}
