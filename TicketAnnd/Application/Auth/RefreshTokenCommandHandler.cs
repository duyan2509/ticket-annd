using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult?>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResult?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenModel = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (tokenModel == null || tokenModel.Expires < DateTime.UtcNow)
            return null;

        await _refreshTokenRepository.RevokeAsync(request.RefreshToken, cancellationToken);

        var user = await _userRepository.GetUserByIdAsync(tokenModel.UserId, cancellationToken);
        if (user == null || !user.IsActive)
            return null;

        var companyRole = tokenModel.CompanyId != Guid.Empty
            ? await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(user.Id, tokenModel.CompanyId, cancellationToken)
            : await _userCompanyRoleRepository.GetFirstByUserIdAsync(user.Id, cancellationToken);

        var companyId = companyRole?.CompanyId ?? tokenModel.CompanyId;
        var role = companyRole?.Role.ToString();

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, companyId, role);
        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CompanyId = companyId,
            Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.AddAsync(newRefresh, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, newRefresh.Token);
    }
}
