using Microsoft.Extensions.Configuration;
using Winzcart.Application.DTOs.Points;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Enums;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class PointsService : IPointsService
{
    private readonly IUserRepository _userRepository;
    private readonly IPointTransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PointsService(
        IUserRepository userRepository,
        IPointTransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PointsBalanceResponse> GetBalanceAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        return new PointsBalanceResponse { Balance = user.PointsBalance };
    }

    public async Task<IEnumerable<PointTransactionResponse>> GetHistoryAsync(Guid userId, int page, int pageSize)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId, page, pageSize);
        
        return transactions.Select(t => new PointTransactionResponse
        {
            Id = t.Id,
            Points = t.Points,
            Type = t.Type.ToString(),
            Description = t.Description,
            CreatedAt = t.CreatedAt
        });
    }

    public async Task AddTransactionAsync(Guid userId, int points, PointTransactionType type, string description)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        if (type == PointTransactionType.Debit && user.PointsBalance < points)
        {
            throw new InsufficientPointsException(points, user.PointsBalance);
        }

        var transaction = new PointTransaction
        {
            UserId = userId,
            Points = points,
            Type = type,
            Description = description
        };

        if (type == PointTransactionType.Credit)
        {
            user.PointsBalance += points;
        }
        else
        {
            user.PointsBalance -= points;
        }

        await _transactionRepository.AddAsync(transaction);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
