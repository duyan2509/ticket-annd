using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Company;

public record GetCompanyMembersQuery(Guid CompanyId, Guid UserId, int Page = 1, int PageSize = 10) : IRequest<MemberPagedResultReadModel?>;

public class GetCompanyMembersQueryHandler : IRequestHandler<GetCompanyMembersQuery, MemberPagedResultReadModel?>
{
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;

    public GetCompanyMembersQueryHandler(IUserCompanyRoleRepository userCompanyRoleRepository)
    {
        _userCompanyRoleRepository = userCompanyRoleRepository;
    }

    public async Task<MemberPagedResultReadModel?> Handle(GetCompanyMembersQuery request, CancellationToken cancellationToken)
    {
        var membership = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.UserId, request.CompanyId, cancellationToken);
        if (membership == null)
            throw new ForbiddenException("You are not a member of this company.");

        return await _userCompanyRoleRepository.GetMembersByCompanyIdAsync(request.CompanyId, request.Page, request.PageSize, cancellationToken);
    }
}
