namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Coupon;

public interface ICouponService
{
    Task<CouponResponse> CreateCouponAsync(Guid merchantUserId, CreateCouponRequest request);
    Task<IEnumerable<CouponResponse>> GetMerchantCouponsAsync(Guid merchantUserId);
    Task<IEnumerable<CouponResponse>> GetAvailableCouponsAsync();
    Task<UserCouponResponse> RedeemCouponAsync(Guid userId, RedeemCouponRequest request);
    Task<IEnumerable<UserCouponResponse>> GetUserCouponsAsync(Guid userId);
    Task<ValidateCouponResponse> ValidateCouponAsync(Guid merchantUserId, ValidateCouponRequest request);
}
