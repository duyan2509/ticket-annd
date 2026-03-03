namespace TicketAnnd.Application.Common;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, Guid companyId, string? role);
    static int AccessTokenExpireMinute = 60;
    static int RefreshTokenExpireDay = 7;

}
