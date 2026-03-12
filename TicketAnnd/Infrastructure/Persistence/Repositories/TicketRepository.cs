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

    public async Task<TicketPagedResultReadModel> GetPagedByCompanyIdAsync(Guid companyId, int page = 1,
        int pageSize = 10, Guid? teamId = null, string? status = null, Guid? categoryId = null, string? qr_str = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var baseQuery = _context.Tickets.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted);
        if (teamId.HasValue)
            baseQuery = baseQuery.Where(x => x.TeamId == teamId.Value);
        if (categoryId.HasValue)
            baseQuery = baseQuery.Where(x => x.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<TicketStatuses>(status, true, out var parsed))
            {
                baseQuery = baseQuery.Where(x => x.Status == parsed);
            }
            else
            {
                baseQuery = baseQuery.Where(x => x.Status.ToString().ToLower() == status.Trim().ToLower());
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

        var items = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TicketReadModel
            {
                Id = t.Id,
                RaiserId = t.RaiserId,
                RaiserName =
                    _context.Users.Where(u => u.Id == t.RaiserId).Select(u => u.Email ?? string.Empty)
                        .FirstOrDefault() ?? string.Empty,
                TeamId = t.TeamId,
                TeamName = t.TeamId == null
                    ? null
                    : _context.Teams.Where(tm => tm.Id == t.TeamId).Select(tm => tm.Name).FirstOrDefault(),
                AssigneeId = t.AssigneeId,
                AssigneeName = t.AssigneeId == null
                    ? null
                    : _context.Users.Where(u => u.Id == t.AssigneeId).Select(u => u.Email).FirstOrDefault(),
                CategoryId = t.CategoryId,
                CategoryName =
                    _context.Categories.Where(c => c.Id == t.CategoryId).Select(c => c.Name ?? string.Empty)
                        .FirstOrDefault() ?? string.Empty,
                Subject = t.Subject,
                TicketCode = t.TicketCode,
                Body = t.Body,
                Status = t.Status.ToString(),
                SlaRuleId = t.SlaRuleId,
                SlaRuleName =
                    _context.SlaRules.Where(r => r.Id == t.SlaRuleId).Select(r => r.Name ?? string.Empty)
                        .FirstOrDefault() ?? string.Empty,
                SlaFirstResponseMinutes = t.SlaFirstResponseMinutes,
                SlaResolutionMinutes = t.SlaResolutionMinutes,
                FirstResponseAt = t.FirstResponseAt == DateTime.MinValue ? null : t.FirstResponseAt,
                IsResolutionBreached = t.IsResolutionBreached,
                IsResponseBreached = t.IsResponseBreached
            })
            .ToListAsync(cancellationToken);

        return new TicketPagedResultReadModel
        {
            Items = items,
            Total = total,
            Page = page,
            Size = pageSize,
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

        var ticket = await _context.Tickets.AsNoTracking()
            .Where(t => !t.IsDeleted && t.CompanyId == companyId && t.TicketCode == ticketCode)
            .Select(t => new TicketReadModel
            {
                Id = t.Id,
                RaiserId = t.RaiserId,
                RaiserName = _context.Users.Where(u => u.Id == t.RaiserId).Select(u => u.Email ?? string.Empty).FirstOrDefault() ?? string.Empty,
                TeamId = t.TeamId,
                TeamName = t.TeamId == null ? null : _context.Teams.Where(tm => tm.Id == t.TeamId).Select(tm => tm.Name).FirstOrDefault(),
                AssigneeId = t.AssigneeId,
                AssigneeName = t.AssigneeId == null ? null : _context.Users.Where(u => u.Id == t.AssigneeId).Select(u => u.Email).FirstOrDefault(),
                CategoryId = t.CategoryId,
                CategoryName = _context.Categories.Where(c => c.Id == t.CategoryId).Select(c => c.Name ?? string.Empty).FirstOrDefault() ?? string.Empty,
                Subject = t.Subject,
                TicketCode = t.TicketCode,
                Body = t.Body,
                Status = t.Status.ToString(),
                SlaRuleId = t.SlaRuleId,
                SlaRuleName = _context.SlaRules.Where(r => r.Id == t.SlaRuleId).Select(r => r.Name ?? string.Empty).FirstOrDefault() ?? string.Empty,
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

