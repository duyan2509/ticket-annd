using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly TicketAnndDbContext _context;

    public InvitationRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Add(invitation);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<InvitationReadModel>> GetByEmailAsync(string email, InviationStatuses? status = null, string? companyName = null, int page = 1, int size = 50, CancellationToken cancellationToken = default)
    {
        var q = _context.Invitations
            .AsNoTracking()
            .Include(i => i.Company)
            .Where(i => i.Email.ToLower() == email.ToLower());

        if (status.HasValue)
            q = q.Where(i => i.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(companyName))
        {
            var cn = companyName.Trim().ToLower();
            q = q.Where(i => i.Company.Name.ToLower().Contains(cn));
        }

        var items = await q
            .OrderByDescending(i => i.Expires)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(i => new InvitationReadModel
            {
                Id = i.Id,
                CompanyId = i.CompanyId,
                CompanyName = i.Company.Name,
                Email = i.Email,
                Status = i.Status,
                Role = i.Role,
                Expires = i.Expires,
                ResponseAt = i.ResponseAt,
                UserId = i.UserId
            })
            .ToListAsync(cancellationToken);

        return items;
    }
}
