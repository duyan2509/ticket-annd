namespace TicketAnnd.Domain.Entities;

public class User:BaseEntity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; } = false;
    public virtual ICollection<UserCompanyRole> UserCompanyRoles { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<UserAgent> UserAgents { get; set; }

}