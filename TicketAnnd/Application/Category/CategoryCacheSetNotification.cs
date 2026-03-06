using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Application.Category;

public record CategoryCacheSetNotification(Guid CompanyId, IReadOnlyList<CategoryReadModel> Data) : INotification;

public class CategoryCacheSetNotificationHandler : INotificationHandler<CategoryCacheSetNotification>
{
    private readonly IDistributedCache _cache;

    public CategoryCacheSetNotificationHandler(IDistributedCache cache)
    {
        _cache = cache;
    }
    private const int CacheExpireHours = 12;
    public async Task Handle(CategoryCacheSetNotification notification, CancellationToken cancellationToken)
    {
        var key = $"categories:{notification.CompanyId}";
        var existing = await _cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(existing)) return;

        var json = JsonSerializer.Serialize(notification.Data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheExpireHours)
        };
        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }
}
