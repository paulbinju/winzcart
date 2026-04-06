using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class PointTransactionRepository : IPointTransactionRepository
{
    private readonly WinzcartDbContext _context;

    public PointTransactionRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PointTransaction>> GetByUserIdAsync(Guid userId, int page, int pageSize)
    {
        return await _context.PointTransactions
            .Where(pt => pt.UserId == userId)
            .OrderByDescending(pt => pt.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _context.PointTransactions.CountAsync(pt => pt.UserId == userId);
    }

    public async Task AddAsync(PointTransaction transaction)
    {
        await _context.PointTransactions.AddAsync(transaction);
    }
}
