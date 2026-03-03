using System.Security.Authentication;
using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
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
    
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetLoginUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new BadRequestException("Invalid email or password.");

        if (!user.IsActive)
            throw new InvalidCredentialException("Account is disabled.");

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new BadRequestException("Invalid email or password.");
       
        var companyId = Guid.Empty;
        string? role;
        if(user.IsAdmin)
            role = nameof(AppRoles.SupperAdmin);
        else
        {
            var firstRole = await _userCompanyRoleRepository.GetFirstByUserIdAsync(user.Id, cancellationToken);
            companyId = firstRole?.CompanyId ?? Guid.Empty;
            role = firstRole?.Role.ToString() ?? nameof(AppRoles.EmptyUser);
        }

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, companyId, role);
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CompanyId = companyId,
            Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(IJwtService.RefreshTokenExpireDay)
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshTokenEntity.Token);
    }
}
