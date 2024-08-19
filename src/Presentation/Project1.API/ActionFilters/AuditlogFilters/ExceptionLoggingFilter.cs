using Microsoft.AspNetCore.Mvc.Filters;
using Project1.Application.Logs;
using Project1.Core.Logs.Entities;
using Project1.Core.Logs.Interfaces;
using System.Diagnostics;

namespace Project1.API.ActionFilters.AuditlogFilters;

public class ExceptionLoggingFilter(IAuditLogService auditLogService) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var resultContext = await next();

            if (resultContext.Exception != null)
            {
                var log = new AuditLog
                {
                    ControllerName = context.RouteData.Values["controller"]?.ToString(),
                    ActionName = context.RouteData.Values["action"]?.ToString(),
                    HttpMethod = context.HttpContext.Request.Method,
                    Timestamp = DateTime.UtcNow,
                    ExecutionDuration = stopwatch.ElapsedMilliseconds,
                    IsException = true,
                    ExceptionMessage = resultContext.Exception.Message,
                    ExceptionStackTrace = resultContext.Exception.StackTrace
                };

                await auditLogService.LogAsync(log);

                // Handle the exception (optional)
                // resultContext.ExceptionHandled = true;
            }
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
