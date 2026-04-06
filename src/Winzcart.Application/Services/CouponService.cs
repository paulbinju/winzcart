using Winzcart.Application.DTOs.Coupon;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Enums;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IUserCouponRepository _userCouponRepository;
    private readonly IMerchantRepository _merchantRepository;
    private readonly IPointsService _pointsService;
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(
        ICouponRepository couponRepository,
        IUserCouponRepository userCouponRepository,
        IMerchantRepository merchantRepository,
        IPointsService pointsService,
        IUnitOfWork unitOfWork)
    {
        _couponRepository = couponRepository;
        _userCouponRepository = userCouponRepository;
        _merchantRepository = merchantRepository;
        _pointsService = pointsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CouponResponse> CreateCouponAsync(Guid merchantUserId, CreateCouponRequest request)
    {
        var merchant = await _merchantRepository.GetByUserIdAsync(merchantUserId);
        if (merchant == null)
            throw new NotFoundException("Merchant", merchantUserId);

        var coupon = new Coupon
        {
            Title = request.Title,
            Description = request.Description,
            PointsRequired = request.PointsRequired,
            ExpiryDate = request.ExpiryDate.ToUniversalTime(),
            MaxRedemptions = request.MaxRedemptions,
            MerchantId = merchant.Id
        };

        await _couponRepository.AddAsync(coupon);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(coupon);
    }

    public async Task<IEnumerable<CouponResponse>> GetMerchantCouponsAsync(Guid merchantUserId)
    {
        var merchant = await _merchantRepository.GetByUserIdAsync(merchantUserId);
        if (merchant == null) return Enumerable.Empty<CouponResponse>();

        var coupons = await _couponRepository.GetByMerchantIdAsync(merchant.Id);
        return coupons.Select(MapToResponse);
    }

    public async Task<IEnumerable<CouponResponse>> GetAvailableCouponsAsync()
    {
        var coupons = await _couponRepository.GetAvailableAsync();
        return coupons.Select(MapToResponse);
    }

    public async Task<UserCouponResponse> RedeemCouponAsync(Guid userId, RedeemCouponRequest request)
    {
        var coupon = await _couponRepository.GetByIdAsync(request.CouponId);
        if (coupon == null)
            throw new NotFoundException("Coupon", request.CouponId);

        if (!coupon.IsActive || coupon.ExpiryDate < DateTime.UtcNow)
            throw new CouponExpiredException();

        if (coupon.RedemptionCount >= coupon.MaxRedemptions)
            throw new CouponFullyRedeemedExceptioin();

        // Will throw InsufficientPointsException inside points service if not enough points
        await _pointsService.AddTransactionAsync(
            userId,
            coupon.PointsRequired,
            PointTransactionType.Debit,
            $"Redeemed coupon: {coupon.Title}");

        // Generate Code
        string uniqueCode = $"CPN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        var userCoupon = new UserCoupon
        {
            CouponId = coupon.Id,
            UserId = userId,
            Code = uniqueCode,
            ExpiryDate = coupon.ExpiryDate
        };

        coupon.RedemptionCount++;

        await _userCouponRepository.AddAsync(userCoupon);
        await _couponRepository.UpdateAsync(coupon);
        await _unitOfWork.SaveChangesAsync();

        return MapToUserCouponResponse(userCoupon, coupon);
    }

    public async Task<IEnumerable<UserCouponResponse>> GetUserCouponsAsync(Guid userId)
    {
        var userCoupons = await _userCouponRepository.GetByUserIdAsync(userId);
        return userCoupons.Select(uc => MapToUserCouponResponse(uc, uc.Coupon));
    }

    public async Task<ValidateCouponResponse> ValidateCouponAsync(Guid merchantUserId, ValidateCouponRequest request)
    {
        var merchant = await _merchantRepository.GetByUserIdAsync(merchantUserId);
        if (merchant == null)
            return new ValidateCouponResponse { IsValid = false, Message = "Merchant not found" };

        var userCoupon = await _userCouponRepository.GetByCodeAsync(request.Code);
        if (userCoupon == null)
            return new ValidateCouponResponse { IsValid = false, Message = "Invalid coupon code" };

        if (userCoupon.Coupon.MerchantId != merchant.Id)
            return new ValidateCouponResponse { IsValid = false, Message = "Coupon does not belong to this merchant" };

        if (userCoupon.IsUsed)
            return new ValidateCouponResponse { IsValid = false, Message = $"Coupon already used on {userCoupon.UsedAt}" };

        if (userCoupon.ExpiryDate < DateTime.UtcNow)
            return new ValidateCouponResponse { IsValid = false, Message = "Coupon has expired" };

        // Mark as used
        userCoupon.IsUsed = true;
        userCoupon.UsedAt = DateTime.UtcNow;
        
        await _userCouponRepository.UpdateAsync(userCoupon);
        await _unitOfWork.SaveChangesAsync();

        return new ValidateCouponResponse
        {
            IsValid = true,
            Message = "Coupon successfully validated and marked as used.",
            CustomerName = userCoupon.User?.Name ?? "Unknown Customer",
            CouponTitle = userCoupon.Coupon.Title
        };
    }

    private CouponResponse MapToResponse(Coupon coupon)
    {
        return new CouponResponse
        {
            Id = coupon.Id,
            Title = coupon.Title,
            Description = coupon.Description,
            PointsRequired = coupon.PointsRequired,
            ExpiryDate = coupon.ExpiryDate,
            MaxRedemptions = coupon.MaxRedemptions,
            RedemptionCount = coupon.RedemptionCount,
            IsActive = coupon.IsActive,
            MerchantId = coupon.MerchantId,
            MerchantName = coupon.Merchant?.BusinessName ?? string.Empty,
            CreatedAt = coupon.CreatedAt
        };
    }

    private UserCouponResponse MapToUserCouponResponse(UserCoupon userCoupon, Coupon coupon)
    {
        return new UserCouponResponse
        {
            Id = userCoupon.Id,
            Code = userCoupon.Code,
            IsUsed = userCoupon.IsUsed,
            ExpiryDate = userCoupon.ExpiryDate,
            RedeemedAt = userCoupon.RedeemedAt,
            UsedAt = userCoupon.UsedAt,
            CouponId = coupon.Id,
            CouponTitle = coupon.Title,
            MerchantName = coupon.Merchant?.BusinessName ?? string.Empty
        };
    }
}
