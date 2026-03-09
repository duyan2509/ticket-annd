namespace TicketAnnd.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; }
    public virtual Company Company { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? LeaderId { get; set; }
    public virtual TeamMember Leader { get; set; }
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public static Team Create(Guid companyId, string name)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = name?.Trim() ?? string.Empty
        };
    }

    public void SetLeader(Guid memberId)
    {
        LeaderId = memberId;
    }
}
