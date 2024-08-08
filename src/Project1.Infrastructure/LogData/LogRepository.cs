using Project1.Core.Generals.Interfaces;
using Project1.Core.Logs.Entities;

namespace Project1.Infrastructure.LogData;

public class LogRepository : ILogRepository
{
    private readonly LogDbContext _context;

    public async Task AddEntity(AuditLog entity)
    {
        await _context.AuditLogs.AddAsync(entity);
    }

    public void Dispose()
    {
        if (_context != null)
        {
            _context.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
