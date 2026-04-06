namespace Winzcart.Application.Interfaces.Services;

using Winzcart.Application.DTOs.Auth;
using Winzcart.Application.DTOs.User;

public interface IUserService
{
    Task<AuthResponse> RegisterCustomerAsync(CustomerRegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserProfileResponse> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
}
