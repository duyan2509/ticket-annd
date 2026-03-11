using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Application.Common;

public record InvalidateOutputCacheNotification(string Tag) : INotification;

public class InvalidateOutputCacheNotificationHandler : INotificationHandler<InvalidateOutputCacheNotification>
{
    private readonly IOutputCacheStore _outputCacheStore;

    public InvalidateOutputCacheNotificationHandler(IOutputCacheStore outputCacheStore)
    {
        _outputCacheStore = outputCacheStore;
    }

    public async Task Handle(InvalidateOutputCacheNotification notification, CancellationToken cancellationToken)
    {
        await _outputCacheStore.EvictByTagAsync(notification.Tag, cancellationToken);
    }
}
