using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class WeeklyWinnerRepository : IWeeklyWinnerRepository
{
    private readonly WinzcartDbContext _context;

    public WeeklyWinnerRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<WeeklyWinner?> GetByIdAsync(Guid id)
    {
        return await _context.WeeklyWinners
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<WeeklyWinner>> GetAllAsync(int page, int pageSize)
    {
        return await _context.WeeklyWinners
            .Include(w => w.User)
            .OrderByDescending(w => w.WeekStartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.WeeklyWinners.CountAsync();
    }

    public async Task<bool> ExistsForWeekAsync(DateTime weekStart, DateTime weekEnd)
    {
        // Simple check if any winner selected whose week overlaps or matches exactly
        return await _context.WeeklyWinners.AnyAsync(w => 
            w.WeekStartDate.Date == weekStart.Date && 
            w.WeekEndDate.Date == weekEnd.Date);
    }

    public async Task AddAsync(WeeklyWinner winner)
    {
        await _context.WeeklyWinners.AddAsync(winner);
    }
}
