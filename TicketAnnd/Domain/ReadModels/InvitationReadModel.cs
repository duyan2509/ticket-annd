using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.ReadModels;

public class InvitationReadModel
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string Email { get; set; }
    public InviationStatuses Status { get; set; }
    public AppRoles Role { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? ResponseAt { get; set; }
    public Guid? UserId { get; set; }
}
