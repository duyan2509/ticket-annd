namespace TicketAnnd.Domain.Entities;

public class Company:BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
    public virtual ICollection<UserCompanyRole> UserCompanyRoles { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; }
}