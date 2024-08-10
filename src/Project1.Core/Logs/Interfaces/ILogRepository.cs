using Project1.Core.Logs.Entities;

namespace Project1.Core.Generals.Interfaces;

public interface ILogRepository : IDisposable, IAsyncDisposable
{
    Task AddEntity(AuditLog entity);
    Task SaveChanges();
}
