using Project1.Core.Generals.Entities;

namespace Project1.Core.Logs.Entities;

public class AuditLog: BaseEntity
{
    // Action-related information
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string? AttrRouteInfo { get; set; }
    public string HttpMethod { get; set; }

    // Request and Response details
    public string? RequestBody { get; set; }
    public string? Parameters { get; set; }
    public string? ResponseBody { get; set; }
    public long RequestSize { get; set; }
    //public long ResponseSize { get; set; }

    // Execution details
    public DateTime Timestamp { get; set; }
    public long ExecutionDuration { get; set; }

    // User and client information
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? QueryString { get; set; }
    public string? Referrer { get; set; }
    //public string? CorrelationId { get; set; }

    // Exception details
    public bool IsException { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionStackTrace { get; set; }
}
