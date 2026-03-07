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
    public async Task<bool> PendingExists(string email, Guid companyId)
    {
        return await _context.Invitations.AnyAsync(i => 
            i.Email == email && 
            i.CompanyId == companyId &&
            i.Status == InviationStatuses.Pending && 
            i.Expires<=DateTime.UtcNow);
    }
    public async Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Update(invitation);
        return Task.CompletedTask;
    }

    public async Task<CompanyInvitationPagedResultReadModel> GetByCompanyIdAsync(Guid companyId, int page = 1, int size = 50, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;

        var baseQuery = _context.Invitations
            .AsNoTracking()
            .Include(i => i.Company)
            .Where(i => i.CompanyId == companyId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(i => i.Expires)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(i => new CompanyInvitationItemReadModel
            {
                Email = i.Email,
                ResponseAt = i.ResponseAt,
                Status = i.Status.ToString(),
                ExpireAt = i.Expires
            })
            .ToListAsync(cancellationToken);

        return new CompanyInvitationPagedResultReadModel
        {
            Items = items,
            Total = total,
            Page = page,
            Size = size
        };
    }

    public async Task<Invitation> GetByEmailAndCompanyIdAsync(string normalizedEmail, Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Invitations
            .FirstOrDefaultAsync(i => i.Email == normalizedEmail && i.CompanyId == companyId, cancellationToken);
    }
}
