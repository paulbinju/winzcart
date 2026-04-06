namespace Winzcart.Domain.Entities;

public class Merchant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BusinessName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    // Navigation
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
