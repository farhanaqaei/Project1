using Microsoft.AspNetCore.Mvc.Filters;
using Project1.Application.Logs;
using Project1.Core.Logs.Entities;
using Project1.Core.Logs.Interfaces;

namespace Project1.API.ActionFilters;

public class AuditLoggingFilter(IAuditLogService auditlogService): ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionContext = context.ActionDescriptor;
        var controllerName = actionContext.DisplayName.Split('.').Last();
        var actionName = actionContext.DisplayName.Split('.').First();
        var httpMethod = context.HttpContext.Request.Method;
        var requestBody = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
        var timestamp = DateTime.UtcNow;

        var resultContext = await next();

        var responseBody = resultContext.HttpContext.Response.StatusCode.ToString();

        var log = new AuditLog
        {
            ControllerName = controllerName,
            ActionName = actionName,
            HttpMethod = httpMethod,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            Timestamp = timestamp
        };

        await auditlogService.LogAsync(log);
    }
}
