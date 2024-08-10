using Project1.Core.Generals.Interfaces;
using Project1.Core.Logs.Entities;
using Project1.Core.Logs.Interfaces;

namespace Project1.Application.Logs;

public class AuditLogService(ILogRepository auditlogRepo) : IAuditLogService
{
    public async Task LogAsync(AuditLog log)
    {
        await auditlogRepo.AddEntity(log);
        await auditlogRepo.SaveChanges();
    }
}
