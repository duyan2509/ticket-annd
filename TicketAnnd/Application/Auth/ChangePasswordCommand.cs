using MediatR;

namespace TicketAnnd.Application.Auth;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Unit>;
