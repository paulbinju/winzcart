using Winzcart.Application.DTOs.Admin;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Domain.Entities;
using Winzcart.Domain.Exceptions;

namespace Winzcart.Application.Services;

public class WinnerService : IWinnerService
{
    private readonly IWeeklyWinnerRepository _winnerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WinnerService(
        IWeeklyWinnerRepository winnerRepository,
        IUserRepository userRepository,
        IBillRepository billRepository,
        IUnitOfWork unitOfWork)
    {
        _winnerRepository = winnerRepository;
        _userRepository = userRepository;
        _billRepository = billRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WeeklyWinnerResponse> SelectWinnerAsync(SelectWinnerRequest request)
    {
        bool alreadySelected = await _winnerRepository.ExistsForWeekAsync(request.WeekStartDate, request.WeekEndDate);
        if (alreadySelected)
            throw new DomainException("A winner has already been selected for this week.");

        Guid selectedUserId;
        bool isManual = false;
        int billsUploaded = 0;
        int pointsEarned = 0;

        if (request.OverrideUserId.HasValue)
        {
            selectedUserId = request.OverrideUserId.Value;
            isManual = true;
            billsUploaded = await _billRepository.CountByUserIdAsync(selectedUserId);
            // Points earned simplified for now
        }
        else
        {
            // Auto selection logic
            // Get all approved bills in the date range
            var start = request.WeekStartDate.ToUniversalTime();
            var end = request.WeekEndDate.ToUniversalTime();
            
            var bills = await _billRepository.GetApprovedBillsInRangeAsync(start, end);
            
            if (!bills.Any())
                throw new DomainException("No approved bills found in the given week range to select a winner.");

            // Find user with most approved bills in that week
            var topUser = bills.GroupBy(b => b.UserId)
                               .Select(g => new { UserId = g.Key, Count = g.Count(), Points = g.Sum(x => x.PointsAwarded) })
                               .OrderByDescending(x => x.Count)
                               .First();

            selectedUserId = topUser.UserId;
            billsUploaded = topUser.Count;
            pointsEarned = topUser.Points;
        }

        var winner = new WeeklyWinner
        {
            UserId = selectedUserId,
            WeekStartDate = request.WeekStartDate.ToUniversalTime(),
            WeekEndDate = request.WeekEndDate.ToUniversalTime(),
            Prize = request.Prize,
            IsManuallySelected = isManual,
            BillsUploaded = billsUploaded,
            PointsEarned = pointsEarned
        };

        await _winnerRepository.AddAsync(winner);
        await _unitOfWork.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(selectedUserId);

        return new WeeklyWinnerResponse
        {
            Id = winner.Id,
            UserId = winner.UserId,
            UserName = user?.Name ?? "Unknown",
            WeekStartDate = winner.WeekStartDate,
            WeekEndDate = winner.WeekEndDate,
            Prize = winner.Prize,
            IsManuallySelected = winner.IsManuallySelected,
            BillsUploaded = winner.BillsUploaded,
            PointsEarned = winner.PointsEarned,
            SelectedAt = winner.SelectedAt
        };
    }

    public async Task<IEnumerable<WeeklyWinnerResponse>> GetWinnersAsync(int page, int pageSize)
    {
        var winners = await _winnerRepository.GetAllAsync(page, pageSize);
        return winners.Select(w => new WeeklyWinnerResponse
        {
            Id = w.Id,
            UserId = w.UserId,
            UserName = w.User?.Name ?? string.Empty,
            WeekStartDate = w.WeekStartDate,
            WeekEndDate = w.WeekEndDate,
            Prize = w.Prize,
            IsManuallySelected = w.IsManuallySelected,
            BillsUploaded = w.BillsUploaded,
            PointsEarned = w.PointsEarned,
            SelectedAt = w.SelectedAt
        });
    }
}
