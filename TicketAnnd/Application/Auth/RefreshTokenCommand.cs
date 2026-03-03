using MediatR;

namespace TicketAnnd.Application.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResult?>;
