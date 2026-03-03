using System.Security.Authentication;
using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class SwitchRoleCommandHandler : IRequestHandler<SwitchRoleCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public SwitchRoleCommandHandler(
        IUserRepository userRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResult> Handle(SwitchRoleCommand request, CancellationToken cancellationToken)
    {
        var companyRole = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.UserId, request.CompanyId, cancellationToken);
        if (companyRole == null)
            throw new InvalidCredentialException("You do not have access to this company.");

        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null || !user.IsActive)
            throw new InvalidCredentialException("User not found or inactive.");

        var role = companyRole.Role.ToString();
        var accessToken = _jwtService.GenerateAccessToken(request.UserId, user.Email, request.CompanyId, role);
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CompanyId = request.CompanyId,
            Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(IJwtService.RefreshTokenExpireDay)
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshTokenEntity.Token);
    }
}
