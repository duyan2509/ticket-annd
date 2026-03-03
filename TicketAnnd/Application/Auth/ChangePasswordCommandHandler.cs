using System.Security.Authentication;
using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetTrackingByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidCredentialException("User not found.");

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidCredentialException("Current password is incorrect.");

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
