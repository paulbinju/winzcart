namespace Winzcart.Application.DTOs.Coupon;

public class CreateCouponRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxRedemptions { get; set; }
}

public class CouponResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxRedemptions { get; set; }
    public int RedemptionCount { get; set; }
    public bool IsActive { get; set; }
    public Guid MerchantId { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RedeemCouponRequest
{
    public Guid CouponId { get; set; }
}

public class UserCouponResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime RedeemedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public Guid CouponId { get; set; }
    public string CouponTitle { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
}

public class ValidateCouponRequest
{
    public string Code { get; set; } = string.Empty;
}

public class ValidateCouponResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CouponTitle { get; set; }
}
