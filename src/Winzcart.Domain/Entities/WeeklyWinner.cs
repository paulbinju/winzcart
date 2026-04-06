namespace Winzcart.Domain.Entities;

public class WeeklyWinner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public string Prize { get; set; } = string.Empty;
    public bool IsManuallySelected { get; set; } = false;
    public int BillsUploaded { get; set; }
    public int PointsEarned { get; set; }
    public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
