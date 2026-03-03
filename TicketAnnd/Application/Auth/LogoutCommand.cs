using MediatR;

namespace TicketAnnd.Application.Auth;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
