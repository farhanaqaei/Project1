using Project1.Core.Logs.Entities;

namespace Project1.Core.Logs.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditLog log);
}
