using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (resetToken == null || resetToken.Expires < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired reset token.");

        var user = await _userRepository.GetTrackingByIdAsync(resetToken.UserId, cancellationToken);
        if (user == null || !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid reset request.");

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _passwordResetTokenRepository.RevokeByUserIdAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
