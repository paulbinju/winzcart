using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly WinzcartDbContext _context;

    public CouponRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await _context.Coupons.Include(c => c.Merchant).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Coupon>> GetByMerchantIdAsync(Guid merchantId)
    {
        return await _context.Coupons
            .Where(c => c.MerchantId == merchantId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Coupon>> GetAvailableAsync()
    {
        return await _context.Coupons
            .Include(c => c.Merchant)
            .Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow && c.RedemptionCount < c.MaxRedemptions)
            .OrderBy(c => c.ExpiryDate)
            .ToListAsync();
    }

    public async Task AddAsync(Coupon coupon)
    {
        await _context.Coupons.AddAsync(coupon);
    }

    public Task UpdateAsync(Coupon coupon)
    {
        _context.Coupons.Update(coupon);
        return Task.CompletedTask;
    }
}
