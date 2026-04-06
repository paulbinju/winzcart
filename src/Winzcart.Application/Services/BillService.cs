using Microsoft.Extensions.Configuration;
using Winzcart.Application.DTOs.Bill;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Enums;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IOcrService _ocrService;
    private readonly IPointsService _pointsService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly int _pointsPer100Rupees;

    public BillService(
        IBillRepository billRepository,
        IOcrService ocrService,
        IPointsService pointsService,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _billRepository = billRepository;
        _ocrService = ocrService;
        _pointsService = pointsService;
        _unitOfWork = unitOfWork;
        
        // Default to 10 points per ₹100 if not configured
        _pointsPer100Rupees = configuration.GetValue<int>("RewardSystem:PointsPer100Rupees", 10);
    }

    public async Task<BillResponse> UploadBillAsync(Guid userId, UploadBillRequest request)
    {
        // 1. In a real scenario, ImageUrl is passed here after being uploaded to storage in Controller
        // 2. OCR processing (mocked)
        var ocrResult = await _ocrService.ExtractAsync(request.ImageUrl);
        
        // Fallback to manual if OCR fails or if manual data provided
        string merchantName = request.MerchantName ?? (ocrResult.Success ? ocrResult.MerchantName : "Unknown");
        decimal amount = request.TotalAmount ?? (ocrResult.Success ? ocrResult.TotalAmount : 0);
        DateTime date = request.BillDate ?? (ocrResult.Success ? ocrResult.BillDate : DateTime.UtcNow);

        // 3. Duplicate checks
        bool isDuplicate = await _billRepository.ExistsDuplicateAsync(userId, merchantName, amount, date);
        if (isDuplicate)
        {
            throw new DuplicateBillException();
        }

        // 4. Calculate potential points
        int pointsAwarded = (int)(amount / 100) * _pointsPer100Rupees;

        var bill = new Bill
        {
            UserId = userId,
            MerchantName = merchantName,
            TotalAmount = amount,
            BillDate = date,
            ImageUrl = request.ImageUrl,
            IsApproved = false, // Must be admin approved
            IsDuplicate = false,
            PointsAwarded = pointsAwarded
        };

        await _billRepository.AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(bill);
    }

    public async Task<BillResponse> ApproveOrRejectBillAsync(Guid billId, ApproveBillRequest request)
    {
        var bill = await _billRepository.GetByIdAsync(billId);
        if (bill == null)
            throw new NotFoundException("Bill", billId);

        if (bill.IsApproved || bill.IsRejected)
        {
            throw new DomainException("Bill has already been processed.");
        }

        if (request.Approved)
        {
            bill.IsApproved = true;
            // Award points
            await _pointsService.AddTransactionAsync(
                bill.UserId,
                bill.PointsAwarded,
                PointTransactionType.Credit,
                $"Points awarded for bill {bill.Id} from {bill.MerchantName}");
        }
        else
        {
            bill.IsRejected = true;
            bill.RejectionReason = request.RejectionReason ?? "Rejected by admin without explicitly stated reason.";
        }

        await _billRepository.UpdateAsync(bill);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(bill);
    }

    public async Task<BillResponse> GetBillByIdAsync(Guid id)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
            throw new NotFoundException("Bill", id);

        return MapToResponse(bill);
    }

    public async Task<IEnumerable<BillResponse>> GetUserBillsAsync(Guid userId, int page, int pageSize)
    {
        var bills = await _billRepository.GetByUserIdAsync(userId, page, pageSize);
        return bills.Select(MapToResponse);
    }

    public async Task<IEnumerable<BillResponse>> GetAllBillsAsync(int page, int pageSize)
    {
        var bills = await _billRepository.GetAllAsync(page, pageSize);
        return bills.Select(MapToResponse);
    }

    private BillResponse MapToResponse(Bill bill)
    {
        return new BillResponse
        {
            Id = bill.Id,
            MerchantName = bill.MerchantName,
            TotalAmount = bill.TotalAmount,
            BillDate = bill.BillDate,
            ImageUrl = bill.ImageUrl,
            IsApproved = bill.IsApproved,
            IsDuplicate = bill.IsDuplicate,
            IsRejected = bill.IsRejected,
            PointsAwarded = bill.PointsAwarded,
            RejectionReason = bill.RejectionReason,
            CreatedAt = bill.CreatedAt,
            UserId = bill.UserId,
            UserName = bill.User?.Name ?? string.Empty
        };
    }
}
