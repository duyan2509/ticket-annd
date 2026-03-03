using MediatR;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Auth;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeResult?>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;

    public GetMeQueryHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
    }

    public async Task<MeResult?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        string currentRole;

        if (!string.IsNullOrEmpty(request.RefreshTokenFromCookie))
        {
            var tokenModel = await _refreshTokenRepository.GetByTokenAsync(request.RefreshTokenFromCookie, cancellationToken);
            if (tokenModel != null && tokenModel.UserId == request.UserId && tokenModel.Expires > DateTime.UtcNow)
            {
                if (tokenModel.CompanyId != Guid.Empty)
                {
                    var companyRole = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.UserId, tokenModel.CompanyId, cancellationToken);
                    currentRole = companyRole?.Role.ToString() ?? nameof(AppRoles.EmptyUser);
                }
                else
                {
                    currentRole = await ResolveRoleWhenNoCompany(user.IsSuperAdmin);
                }
            }
            else
            {
                currentRole = await ResolveRoleWhenNoCompany(user.IsSuperAdmin);
            }
        }
        else
        {
            currentRole = await ResolveRoleWhenNoCompany(user.IsSuperAdmin);
        }

        return new MeResult(user.Id, user.Email, user.IsActive, currentRole);
    }

    private static Task<string> ResolveRoleWhenNoCompany(bool isSuperAdmin)
    {
        return Task.FromResult(isSuperAdmin ? nameof(AppRoles.SupperAdmin) : nameof(AppRoles.EmptyUser));
    }
}
