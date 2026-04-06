namespace Winzcart.Domain.Entities;

public class Coupon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PointsRequired { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxRedemptions { get; set; }
    public int RedemptionCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid MerchantId { get; set; }
    public Merchant Merchant { get; set; } = null!;

    // Navigation
    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
}
