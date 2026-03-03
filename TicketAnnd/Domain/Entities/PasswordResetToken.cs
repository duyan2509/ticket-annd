namespace TicketAnnd.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
