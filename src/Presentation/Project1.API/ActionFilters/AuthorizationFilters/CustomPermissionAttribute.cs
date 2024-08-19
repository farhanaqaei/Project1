using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project1.Core.Generals.Interfaces;
using Project1.Infrastructure.Cache;
using System.Net.Http;
using System.Security.Claims;

namespace Project1.API.ActionFilters.AuthorizationFilters;

public class CustomPermissionAttribute(string permission): Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ForbidResult();
            return;
        }

        bool hasPermission = false;

        // Option 1: Check permission from token claims
        hasPermission = user.Claims.Any(c => c.Type == "Permission" && c.Value == permission);

        // Option 2: Check permission from cache
        //var cacheManager = context.HttpContext.RequestServices.GetService<ICacheManager>();

        //var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //if (userId != null)
        //{
        //    var cacheKey = $"Permissions_User_{userId}";
        //    var cachedPermissions = cacheManager.Get<List<string>>(cacheKey);

        //    if (cachedPermissions != null && cachedPermissions.Contains(permission))
        //    {
        //        hasPermission = true;
        //    }
        //}

        if (!hasPermission)

        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
