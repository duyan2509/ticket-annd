using MediatR;

namespace TicketAnnd.Application.Company;

public record CreateCompanyCommand(Guid UserId, string Name) : IRequest<CreateCompanyResult>;

public record CreateCompanyResult(Guid CompanyId, string Name);
