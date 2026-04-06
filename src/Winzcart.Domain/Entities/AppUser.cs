using Winzcart.Domain.Enums;

namespace Winzcart.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public int PointsBalance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Merchant? Merchant { get; set; }
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
    public ICollection<WeeklyWinner> WeeklyWins { get; set; } = new List<WeeklyWinner>();
}
