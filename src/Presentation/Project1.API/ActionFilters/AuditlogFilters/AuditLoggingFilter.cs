using Microsoft.AspNetCore.Mvc.Filters;
using Project1.Application.Logs;
using Project1.Core.Logs.Entities;
using Project1.Core.Logs.Interfaces;
using System.Diagnostics;

namespace Project1.API.ActionFilters.AuditlogFilters;

public class AuditLoggingFilter(IAuditLogService auditLogService) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
        var userId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.Identity.Name : "Anonymous";
        var requestPath = context.HttpContext.Request.Path;
        var queryString = context.HttpContext.Request.QueryString.ToString();
        var referrer = context.HttpContext.Request.Headers["Referer"].ToString();
        //var correlationId = context.HttpContext.Request.Headers["X-Correlation-ID"].ToString() ?? Guid.NewGuid().ToString();
        var requestSize = context.HttpContext.Request.ContentLength ?? 0;

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
        //var responseSize = resultContext.HttpContext.Response.ContentLength ?? 0;

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
            ExecutionDuration = stopwatch.ElapsedMilliseconds,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            UserId = userId,
            RequestPath = requestPath,
            QueryString = queryString,
            Referrer = referrer,
            //CorrelationId = correlationId,
            RequestSize = requestSize,
            //ResponseSize = responseSize
        };

        await auditLogService.LogAsync(log);
    }
}
