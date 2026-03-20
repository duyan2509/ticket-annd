using MediatR;
using Microsoft.Extensions.Options;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Options;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FrontendOptions _frontendOptions;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork,
        IOptions<FrontendOptions> frontendOptions)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
        _frontendOptions = frontendOptions.Value;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var fe_base_url = _frontendOptions.BaseUrl;
        var user = await _userRepository.GetLoginUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return Unit.Value; // Don't reveal whether email exists

        await _passwordResetTokenRepository.RevokeByUserIdAsync(user.Id, cancellationToken);

        var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        await _passwordResetTokenRepository.AddAsync(resetToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var resetLink = $"{fe_base_url}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(request.Email)}";
        var body = $@"<p>You requested a password reset.</p><p><a href=""{resetLink}"">Reset password</a></p><p>This link expires in 1 hour. If you didn't request this, ignore this email.</p>";
        await _emailSender.SendAsync(request.Email, "Reset your password", body, cancellationToken);

        return Unit.Value;
    }
}
