namespace TicketAnnd.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; } = false;
    public virtual ICollection<UserCompanyRole> UserCompanyRoles { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<TeamMember> TeamMembers { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }

}