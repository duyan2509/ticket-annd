using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace TicketAnnd.Application.Category;
public record CategoryChangeNotification(Guid CompanyId) : INotification;

public class CategoryChangeNotificationHandler:INotificationHandler<CategoryChangeNotification>
{
    private readonly IDistributedCache _cache;

    public CategoryChangeNotificationHandler(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task Handle(CategoryChangeNotification notification, CancellationToken cancellationToken)
    {
        var key = $"categories:{notification.CompanyId}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}