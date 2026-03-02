namespace TicketAnnd.Domain.Entities;

public class RefreshToken:BaseEntity
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid CompanyId { get; set; }
}