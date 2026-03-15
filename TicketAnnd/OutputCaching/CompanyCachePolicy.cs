using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace TicketAnnd.OutputCaching;

public sealed class CompanyCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        context.CacheVaryByRules.QueryKeys = "*";

        var companyId = context.HttpContext.User?.FindFirst("company_id")?.Value ?? string.Empty;
        context.CacheVaryByRules.VaryByValues["company"] = companyId;
        context.CacheVaryByRules.VaryByValues["companyId"] = companyId;

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

        // add company-scoped tags based on request path so we can evict per-company
        var companyId = context.HttpContext.User?.FindFirst("company_id")?.Value ?? string.Empty;
        if (!string.IsNullOrEmpty(companyId))
        {
            var path = context.HttpContext.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/api/categories", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"company:{companyId}:categories");
            }
            else if (path.StartsWith("/api/teams", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"company:{companyId}:teams");
            }
            else if (path.StartsWith("/api/sla", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"company:{companyId}:sla");
            }
            else if (path.StartsWith("/api/invitations/company", StringComparison.OrdinalIgnoreCase))
            {
                context.Tags.Add($"company:{companyId}:invitations");
            }
        }

        return ValueTask.CompletedTask;
    }
}
