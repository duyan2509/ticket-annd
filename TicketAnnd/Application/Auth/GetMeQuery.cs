using MediatR;

namespace TicketAnnd.Application.Auth;

public record GetMeQuery(Guid UserId) : IRequest<MeResult?>;

public record MeResult(Guid Id, string Email, bool IsActive);
