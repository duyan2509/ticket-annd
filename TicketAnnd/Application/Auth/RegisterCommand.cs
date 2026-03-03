using MediatR;

namespace TicketAnnd.Application.Auth;

public record RegisterCommand(string Email, string Password) : IRequest<Unit>;
