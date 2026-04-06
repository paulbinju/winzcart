using Winzcart.Domain.Enums;

namespace Winzcart.Domain.Entities;

public class PointTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Points { get; set; }
    public PointTransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
