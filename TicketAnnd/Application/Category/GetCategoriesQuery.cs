using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TicketAnnd.Application.Category;

public record GetCategoriesQuery(Guid CompanyId) : IRequest<IReadOnlyList<CategoryReadModel>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryReadModel>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDistributedCache _cache;
    private readonly IMediator _mediator;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IDistributedCache cache, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<IReadOnlyList<CategoryReadModel>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var key = $"categories:{request.CompanyId}";
        var cached = await _cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<CategoryReadModel>>(cached);
                if (list != null) return list;
            }
            catch
            {
            }
        }

        var data = await _categoryRepository.GetAllByCompanyIdAsync(request.CompanyId, cancellationToken);

        await _mediator.Publish(new CategoryCacheSetNotification(request.CompanyId, data), cancellationToken);

        return data;
    }
}
