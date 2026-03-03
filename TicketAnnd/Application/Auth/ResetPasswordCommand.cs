using MediatR;

namespace TicketAnnd.Application.Auth;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Unit>;
