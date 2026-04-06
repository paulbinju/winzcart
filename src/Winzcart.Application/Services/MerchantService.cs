using Winzcart.Application.DTOs.Auth;
using Winzcart.Application.DTOs.Merchant;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Enums;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class MerchantService : IMerchantService
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public MerchantService(
        IMerchantRepository merchantRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _merchantRepository = merchantRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterMerchantAsync(MerchantRegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new DomainException("Email is already in use.");

        // In production, use proper password hashing (e.g. BCrypt)
        // This is simplified. Infrastructure layer could handle this.
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new AppUser
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Merchant
        };

        var merchant = new Merchant
        {
            UserId = user.Id,
            BusinessName = request.BusinessName,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            IsVerified = false // Needs admin verification
        };

        await _userRepository.AddAsync(user);
        await _merchantRepository.AddAsync(merchant);
        await _unitOfWork.SaveChangesAsync();

        string token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            Role = user.Role.ToString(),
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<MerchantProfileResponse> GetProfileAsync(Guid userId)
    {
        var merchant = await _merchantRepository.GetByUserIdAsync(userId);
        if (merchant == null)
            throw new NotFoundException("Merchant profile", userId);

        return new MerchantProfileResponse
        {
            Id = merchant.Id,
            BusinessName = merchant.BusinessName,
            ContactPhone = merchant.ContactPhone,
            Address = merchant.Address,
            IsVerified = merchant.IsVerified,
            UserId = merchant.UserId,
            OwnerName = merchant.User?.Name ?? string.Empty,
            Email = merchant.User?.Email ?? string.Empty,
            CreatedAt = merchant.CreatedAt
        };
    }
}
