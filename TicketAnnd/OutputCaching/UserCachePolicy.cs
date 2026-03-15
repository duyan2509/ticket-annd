using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace TicketAnnd.OutputCaching;

public sealed class UserCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        context.CacheVaryByRules.QueryKeys = "*";

        var userId = context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? context.HttpContext.User?.FindFirst("sub")?.Value
                 ?? string.Empty;
        context.CacheVaryByRules.VaryByValues["user"] = userId;
        context.CacheVaryByRules.VaryByValues["userId"] = userId;

        context.ResponseExpirationTimeSpan = TimeSpan.FromSeconds(60);

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        if (response.StatusCode != StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        var userId = context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? context.HttpContext.User?.FindFirst("sub")?.Value
                     ?? string.Empty;
        if (!string.IsNullOrEmpty(userId))
        {
            var path = context.HttpContext.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/api/auth/me", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"Users:{userId}");
            }
            else if (path.StartsWith("/api/invitations/me", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"Users:{userId}:invitations");
            }
        }

        return ValueTask.CompletedTask;
    }
}
