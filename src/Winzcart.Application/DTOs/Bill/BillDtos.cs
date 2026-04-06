namespace Winzcart.Application.DTOs.Bill;

public class UploadBillRequest
{
    // IFormFile is in the API layer; here we pass already-stored image URL + optional manual data
    public string? MerchantName { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? BillDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class BillResponse
{
    public Guid Id { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime BillDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsRejected { get; set; }
    public int PointsAwarded { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class ApproveBillRequest
{
    public bool Approved { get; set; }
    public string? RejectionReason { get; set; }
}
