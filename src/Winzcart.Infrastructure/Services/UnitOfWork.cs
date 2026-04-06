using Winzcart.Application.Interfaces.Services;
using Winzcart.Infrastructure.Data;

namespace Winzcart.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly WinzcartDbContext _context;

    public UnitOfWork(WinzcartDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
