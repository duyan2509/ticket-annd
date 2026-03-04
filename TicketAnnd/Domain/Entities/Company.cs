using TicketAnnd.Application;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class Company:BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
    public virtual ICollection<UserCompanyRole> UserCompanyRoles { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; }
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; }
    public static Company Create(string name, Guid ownerId)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = ownerId
        };

        company.AddUser(ownerId, AppRoles.CompanyAdmin);

        return company;
    }

    public void AddUser(Guid userId, AppRoles role)
    {
        if (UserCompanyRoles.Any(x => x.UserId == userId))
            throw new BusinessException("User already in company");

        UserCompanyRoles.Add(new UserCompanyRole(
            Guid.NewGuid(),
            Id,
            userId,
            role
        ));
    }

}