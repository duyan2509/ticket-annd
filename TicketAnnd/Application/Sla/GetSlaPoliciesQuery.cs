using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Sla;

public record GetSlaPoliciesQuery(Guid CompanyId, int Page = 1, int PageSize = 10) : IRequest<SlaPolicyPagedResultReadModel>;

public class GetSlaPoliciesQueryHandler : IRequestHandler<GetSlaPoliciesQuery, SlaPolicyPagedResultReadModel>
{
    private readonly ISlaPolicyRepository _repo;

    public GetSlaPoliciesQueryHandler(ISlaPolicyRepository repo)
    {
        _repo = repo;
    }

    public async Task<SlaPolicyPagedResultReadModel> Handle(GetSlaPoliciesQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetPagedByCompanyIdAsync(request.CompanyId, request.Page, request.PageSize, cancellationToken);
    }
}
