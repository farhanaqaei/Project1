using Project1.Core.Generals.Entities;

namespace Project1.Core.Logs.Entities;

public class AuditLog: BaseEntity
{
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string? AttrRouteInfo { get; set; }
    public string HttpMethod { get; set; }
    public string? RequestBody { get; set; }
    public string? Parameters { get; set; }
    public string? ResponseBody { get; set; }
    public DateTime Timestamp { get; set; }
    public long ExecutionDuration { get; set; }

    public bool IsException { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionStackTrace { get; set; }
}
