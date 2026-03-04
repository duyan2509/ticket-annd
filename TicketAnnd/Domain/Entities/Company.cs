using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using TicketAnnd.Application;
using TicketAnnd.Domain.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TicketAnnd.Domain.Entities;

public class Company:BaseEntity
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
    public virtual ICollection<UserCompanyRole> UserCompanyRoles { get; set; } = new  List<UserCompanyRole>();
    public virtual ICollection<Ticket> Tickets { get; set; }
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; }
    public static Company Create(string name, Guid ownerId)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = ownerId,
            Slug = GenerateSlug(name)
        };

        company.AddUser(ownerId, AppRoles.CompanyAdmin);

        return company;
    }

    private static string GenerateSlug(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;

        var str = phrase.Trim().ToLowerInvariant();

        str = str.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in str)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }
        str = sb.ToString().Normalize(NormalizationForm.FormC);

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-");

        str = Regex.Replace(str, "-{2,}", "-");

        return str;
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