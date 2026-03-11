using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Application.Category;
public record CategoryChangeNotification(Guid CompanyId) : INotification;

public class CategoryChangeNotificationHandler:INotificationHandler<CategoryChangeNotification>
{
    private readonly IDistributedCache _cache;
    private readonly IOutputCacheStore _outputCacheStore;

    public CategoryChangeNotificationHandler(IDistributedCache cache, IOutputCacheStore outputCacheStore)
    {
        _cache = cache;
        _outputCacheStore = outputCacheStore;
    }
    public async Task Handle(CategoryChangeNotification notification, CancellationToken cancellationToken)
    {
        var key = $"categories:{notification.CompanyId}";
        await _cache.RemoveAsync(key, cancellationToken);
        await _outputCacheStore.EvictByTagAsync("Categories", cancellationToken);
    }
}