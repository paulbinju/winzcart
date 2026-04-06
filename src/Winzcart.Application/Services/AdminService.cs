using Winzcart.Application.DTOs.Admin;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IMerchantRepository _merchantRepository;
    private readonly IBillRepository _billRepository;
    private readonly ICouponRepository _couponRepository;

    public AdminService(
        IUserRepository userRepository,
        IMerchantRepository merchantRepository,
        IBillRepository billRepository,
        ICouponRepository couponRepository)
    {
        _userRepository = userRepository;
        _merchantRepository = merchantRepository;
        _billRepository = billRepository;
        _couponRepository = couponRepository;
    }

    public async Task<AnalyticsResponse> GetAnalyticsAsync()
    {
        var usersCount = await _userRepository.CountAsync();
        var merchantsCount = await _merchantRepository.CountAsync();
        var billsCount = await _billRepository.CountAsync();
        
        // Simplified metrics. In production, these would be specific queries.
        return new AnalyticsResponse
        {
            TotalUsers = usersCount,
            TotalMerchants = merchantsCount,
            TotalBillsUploaded = billsCount,
            TotalBillsApproved = 0, // Placeholder
            TotalCouponsCreated = 0, // Placeholder
            TotalCouponsRedeemed = 0 // Placeholder
        };
    }
}
