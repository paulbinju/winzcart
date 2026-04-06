namespace Winzcart.Application.DTOs.Admin;

public class AnalyticsResponse
{
    public int TotalUsers { get; set; }
    public int TotalMerchants { get; set; }
    public int TotalBillsUploaded { get; set; }
    public int TotalBillsApproved { get; set; }
    public int TotalCouponsCreated { get; set; }
    public int TotalCouponsRedeemed { get; set; }
}

public class WeeklyWinnerResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public string Prize { get; set; } = string.Empty;
    public bool IsManuallySelected { get; set; }
    public int BillsUploaded { get; set; }
    public int PointsEarned { get; set; }
    public DateTime SelectedAt { get; set; }
}

public class SelectWinnerRequest
{
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public string Prize { get; set; } = string.Empty;
    public Guid? OverrideUserId { get; set; } // If admin wants to force a specific user
}
