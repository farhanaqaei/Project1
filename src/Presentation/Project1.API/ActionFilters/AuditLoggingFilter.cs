using Microsoft.AspNetCore.Mvc.Filters;
using Project1.Application.Logs;
using Project1.Core.Logs.Entities;
using Project1.Core.Logs.Interfaces;
using System.Diagnostics;

namespace Project1.API.ActionFilters;

public class AuditLoggingFilter(IAuditLogService auditLogService) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        var actionContext = context.RouteData;
        var controllerName = actionContext?.Values["controller"]?.ToString();
        var actionName = actionContext?.Values["action"]?.ToString();
        var attrRouteInfo = context.ActionDescriptor?.AttributeRouteInfo?.Name;
        var httpMethod = context.HttpContext.Request.Method;
        var requestBody = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
        var parameters = context.ActionArguments;
        var timestamp = DateTime.UtcNow;

        var resultContext = await next();

        if (resultContext.Exception != null)
        {
            return;
        }

        var responseBody = resultContext.HttpContext.Response.StatusCode.ToString();

        stopwatch.Stop();

        var log = new AuditLog
        {
            ControllerName = controllerName,
            ActionName = actionName,
            AttrRouteInfo = attrRouteInfo,
            HttpMethod = httpMethod,
            RequestBody = requestBody,
            Parameters = string.Join(", ", parameters),
            ResponseBody = responseBody,
            Timestamp = timestamp,
            ExecutionDuration = stopwatch.ElapsedMilliseconds
        };

        await auditLogService.LogAsync(log);
    }
}
