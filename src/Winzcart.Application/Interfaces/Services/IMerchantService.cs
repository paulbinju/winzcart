namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Auth;
using Winzcart.Application.DTOs.Merchant;

public interface IMerchantService
{
    Task<AuthResponse> RegisterMerchantAsync(MerchantRegisterRequest request);
    Task<MerchantProfileResponse> GetProfileAsync(Guid userId);
}
