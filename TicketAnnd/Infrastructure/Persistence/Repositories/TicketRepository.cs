using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly TicketAnndDbContext _context;

    public TicketRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

public async Task<TicketPagedResultReadModel> GetPagedByCompanyIdAsync(
    Guid companyId,
    int page = 1,
    int pageSize = 10,
    Guid? teamId = null,
    string? status = null,
    Guid? categoryId = null,
    string? qr_str = null,
    bool? unassigned = null,
    CancellationToken cancellationToken = default)
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 10;

    var baseQuery = _context.Tickets
        .AsNoTracking()
        .Where(x => x.CompanyId == companyId && !x.IsDeleted);

    if (unassigned.HasValue && unassigned.Value)
        baseQuery = baseQuery.Where(x => x.TeamId == null);
    else if (teamId.HasValue)
        baseQuery = baseQuery.Where(x => x.TeamId == teamId.Value);

    if (categoryId.HasValue)
        baseQuery = baseQuery.Where(x => x.CategoryId == categoryId.Value);

    if (!string.IsNullOrWhiteSpace(status))
    {
        if (Enum.TryParse<TicketStatuses>(status, true, out var parsed))
        {
            baseQuery = baseQuery.Where(x => x.Status == parsed);
        }
    }

    if (!string.IsNullOrWhiteSpace(qr_str))
    {
        var s = qr_str.Trim().ToLower();

        baseQuery = baseQuery.Where(t =>
            EF.Functions.ToTsVector("english",
                    (t.Subject ?? "") + " " + (t.Body ?? "")
                )
                .Matches(EF.Functions.PlainToTsQuery("english", s)));
    }

    var total = await baseQuery.CountAsync(cancellationToken);

    var query =
        from t in baseQuery

        join r in _context.Users on t.RaiserId equals r.Id into raiser
        from r in raiser.DefaultIfEmpty()

        join a in _context.Users on t.AssigneeId equals a.Id into assignee
        from a in assignee.DefaultIfEmpty()

        join tm in _context.Teams on t.TeamId equals tm.Id into team
        from tm in team.DefaultIfEmpty()

        join c in _context.Categories on t.CategoryId equals c.Id into category
        from c in category.DefaultIfEmpty()

        join s in _context.SlaRules on t.SlaRuleId equals s.Id into sla
        from s in sla.DefaultIfEmpty()

        orderby t.CreatedAt descending

        select new TicketReadModel
        {
            Id = t.Id,
            RaiserId = t.RaiserId,
            RaiserName = r.Email ?? string.Empty,

            TeamId = t.TeamId,
            TeamName = tm.Name,

            AssigneeId = t.AssigneeId,
            AssigneeName = a.Email,

            CategoryId = t.CategoryId,
            CategoryName = c.Name ?? string.Empty,

            Subject = t.Subject,
            TicketCode = t.TicketCode,
            Body = t.Body,
            Status = t.Status.ToString(),

            SlaRuleId = t.SlaRuleId,
            SlaRuleName = s.Name ?? string.Empty,

            SlaFirstResponseMinutes = t.SlaFirstResponseMinutes,
            SlaResolutionMinutes = t.SlaResolutionMinutes,

            FirstResponseAt = t.FirstResponseAt == DateTime.MinValue ? null : t.FirstResponseAt,

            IsResolutionBreached = t.IsResolutionBreached,
            IsResponseBreached = t.IsResponseBreached
        };

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return new TicketPagedResultReadModel
    {
        Items = items,
        Total = total,
        Page = page,
        Size = pageSize
    };
}


    public async Task<Ticket?> GetTrackingByIdCompanyIdAsync(Guid requestTicketId, Guid CompanyId, CancellationToken cancellationToken)
    {
        return await  _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == requestTicketId && x.CompanyId==CompanyId, cancellationToken);
    }

    public Task UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        return Task.CompletedTask;
    }

    public async Task<TicketReadModel?> GetByCompanyIdAndCodeAsync(Guid companyId, string ticketCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticketCode)) return null;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var ticket = await _context.Tickets.AsNoTracking()
            .Where(t => !t.IsDeleted && t.CompanyId == companyId && t.TicketCode == ticketCode)
            .Include(t=>t.Raiser)
            .Include(t=>t.Team)
            .Include(t=>t.Assignee)
            .Include(t=>t.Category)
            .Include(t=>t.SlaRule)
            .Select(t => new TicketReadModel
            {
                Id = t.Id,
                RaiserId = t.RaiserId,
                RaiserName = t.Raiser.Email ?? string.Empty,
                TeamId = t.TeamId??Guid.Empty,
                TeamName = t.Team.Name ?? string.Empty,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee.Email ?? string.Empty,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name ?? string.Empty,
                Subject = t.Subject,
                TicketCode = t.TicketCode,
                Body = t.Body,
                Status = t.Status.ToString(),
                SlaRuleId = t.SlaRuleId,
                SlaRuleName = t.SlaRule.Name ?? string.Empty    ,
                SlaFirstResponseMinutes = t.SlaFirstResponseMinutes,
                SlaResolutionMinutes = t.SlaResolutionMinutes,
                FirstResponseAt = t.FirstResponseAt == DateTime.MinValue ? null : t.FirstResponseAt,
                IsResolutionBreached = t.IsResolutionBreached,
                IsResponseBreached = t.IsResponseBreached
            })
            .FirstOrDefaultAsync(cancellationToken);

        return ticket;
    }
}

