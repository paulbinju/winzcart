namespace Winzcart.Domain.Entities;

public class UserCoupon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public DateTime ExpiryDate { get; set; }
    public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }

    // FK
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid CouponId { get; set; }
    public Coupon Coupon { get; set; } = null!;
}
