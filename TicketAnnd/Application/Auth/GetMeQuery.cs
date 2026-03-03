using MediatR;

namespace TicketAnnd.Application.Auth;

public record GetMeQuery(Guid UserId, string? RefreshTokenFromCookie) : IRequest<MeResult?>;

public record MeResult(Guid Id, string Email, bool IsActive, string CurrentRole);
