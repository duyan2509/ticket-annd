using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Company;


public record GetCompaniesQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<CompanyPagedResultReadModel>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, CompanyPagedResultReadModel>
{
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;

    public GetCompaniesQueryHandler(IUserCompanyRoleRepository userCompanyRoleRepository)
    {
        _userCompanyRoleRepository = userCompanyRoleRepository;
    }

    public async Task<CompanyPagedResultReadModel> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _userCompanyRoleRepository.GetCompanyOptionsPagedByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
    }
}
