using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly WinzcartDbContext _context;

    public BillRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<Bill?> GetByIdAsync(Guid id)
    {
        return await _context.Bills.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Bill>> GetByUserIdAsync(Guid userId, int page, int pageSize)
    {
        return await _context.Bills
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Bills
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Bills.CountAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _context.Bills.CountAsync(b => b.UserId == userId && b.IsApproved);
    }

    public async Task<bool> ExistsDuplicateAsync(Guid userId, string merchantName, decimal amount, DateTime date)
    {
        return await _context.Bills.AnyAsync(b => 
            b.UserId == userId && 
            b.MerchantName.ToLower() == merchantName.ToLower() && 
            b.TotalAmount == amount && 
            b.BillDate.Date == date.Date);
    }

    public async Task<IEnumerable<Bill>> GetApprovedBillsInRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Bills
            .Where(b => b.IsApproved && b.BillDate >= start && b.BillDate <= end)
            .ToListAsync();
    }

    public async Task AddAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);
    }

    public Task UpdateAsync(Bill bill)
    {
        _context.Bills.Update(bill);
        return Task.CompletedTask;
    }
}
