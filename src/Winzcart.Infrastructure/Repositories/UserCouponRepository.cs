using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class UserCouponRepository : IUserCouponRepository
{
    private readonly WinzcartDbContext _context;

    public UserCouponRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<UserCoupon?> GetByIdAsync(Guid id)
    {
        return await _context.UserCoupons
            .Include(uc => uc.Coupon)
            .ThenInclude(c => c.Merchant)
            .FirstOrDefaultAsync(uc => uc.Id == id);
    }

    public async Task<UserCoupon?> GetByCodeAsync(string code)
    {
        return await _context.UserCoupons
            .Include(uc => uc.Coupon)
            .Include(uc => uc.User)
            .FirstOrDefaultAsync(uc => uc.Code == code);
    }

    public async Task<IEnumerable<UserCoupon>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserCoupons
            .Include(uc => uc.Coupon)
            .ThenInclude(c => c.Merchant)
            .Where(uc => uc.UserId == userId)
            .OrderByDescending(uc => uc.RedeemedAt)
            .ToListAsync();
    }

    public async Task AddAsync(UserCoupon userCoupon)
    {
        await _context.UserCoupons.AddAsync(userCoupon);
    }

    public Task UpdateAsync(UserCoupon userCoupon)
    {
        _context.UserCoupons.Update(userCoupon);
        return Task.CompletedTask;
    }
}
