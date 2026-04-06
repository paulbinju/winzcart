namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Bill;

public interface IBillService
{
    Task<BillResponse> UploadBillAsync(Guid userId, UploadBillRequest request);
    Task<BillResponse> GetBillByIdAsync(Guid id);
    Task<IEnumerable<BillResponse>> GetUserBillsAsync(Guid userId, int page, int pageSize);
    Task<IEnumerable<BillResponse>> GetAllBillsAsync(int page, int pageSize);
    Task<BillResponse> ApproveOrRejectBillAsync(Guid billId, ApproveBillRequest request);
}
