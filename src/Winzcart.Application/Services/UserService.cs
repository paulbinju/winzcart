using Winzcart.Application.DTOs.Auth;
using Winzcart.Application.DTOs.User;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Enums;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterCustomerAsync(CustomerRegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new DomainException("Email is already in use.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new AppUser
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Customer
        };

        await _userRepository.AddAsync(user);
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

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedException("Invalid email or password.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is disabled.");

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

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User profile", userId);

        return new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            PointsBalance = user.PointsBalance,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User profile", userId);

        user.Name = request.Name;
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
