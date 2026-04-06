namespace Winzcart.Domain.Entities;

public class Bill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MerchantName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime BillDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    public bool IsDuplicate { get; set; } = false;
    public bool IsRejected { get; set; } = false;
    public int PointsAwarded { get; set; } = 0;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
