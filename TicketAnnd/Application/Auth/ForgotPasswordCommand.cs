using MediatR;

namespace TicketAnnd.Application.Auth;

public record ForgotPasswordCommand(string Email) : IRequest<Unit>;
