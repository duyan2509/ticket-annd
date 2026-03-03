using MediatR;

namespace TicketAnnd.Application.Auth;

public record SwitchRoleCommand(Guid UserId, Guid CompanyId) : IRequest<LoginResult>;
