using Microsoft.EntityFrameworkCore;
using Project1.Core.Logs.Entities;

namespace Project1.Infrastructure.LogData;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; }
}
