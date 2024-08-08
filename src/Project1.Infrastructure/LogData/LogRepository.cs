using Project1.Core.Generals.Interfaces;
using Project1.Core.Logs.Entities;

namespace Project1.Infrastructure.LogData;

public class LogRepository(LogDbContext context) : ILogRepository
{

    public async Task AddEntity(AuditLog entity)
    {
        await context.AuditLogs.AddAsync(entity);
    }

    public void Dispose()
    {
        if (context != null)
        {
            context.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (context != null)
        {
            await context.DisposeAsync();
        }
    }

    public async Task SaveChanges()
    {
        await context.SaveChangesAsync();
    }
}
