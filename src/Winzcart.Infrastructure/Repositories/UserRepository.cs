using Microsoft.EntityFrameworkCore;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Domain.Entities;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WinzcartDbContext _context;

    public UserRepository(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task AddAsync(AppUser user)
    {
        await _context.Users.AddAsync(user);
    }

    public Task UpdateAsync(AppUser user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }
}
