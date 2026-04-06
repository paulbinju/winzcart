using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class MerchantRepository : IMerchantRepository
{
    private readonly WinzcartDbContext _context;

    public MerchantRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<Merchant?> GetByIdAsync(Guid id)
    {
        return await _context.Merchants.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Merchant?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Merchants.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<IEnumerable<Merchant>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Merchants
            .Include(m => m.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Merchants.CountAsync();
    }

    public async Task AddAsync(Merchant merchant)
    {
        await _context.Merchants.AddAsync(merchant);
    }

    public Task UpdateAsync(Merchant merchant)
    {
        _context.Merchants.Update(merchant);
        return Task.CompletedTask;
    }
}
