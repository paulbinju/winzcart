namespace Winzcart.Application.DTOs.Points;

public class PointTransactionResponse
{
    public Guid Id { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PointsBalanceResponse
{
    public int Balance { get; set; }
}
