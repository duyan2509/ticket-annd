using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class SlaPolicyRepository : ISlaPolicyRepository
{
    private readonly TicketAnndDbContext _context;

    public SlaPolicyRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(SlaPolicy policy, CancellationToken cancellationToken = default)
    {
        _context.SlaPolicies.Add(policy);
        return Task.CompletedTask;
    }

    public async Task<SlaPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SlaPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(SlaPolicy policy, CancellationToken cancellationToken = default)
    {
        _context.SlaPolicies.Update(policy);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SlaPolicy policy, CancellationToken cancellationToken = default)
    {
        _context.SlaPolicies.Remove(policy);
        return Task.CompletedTask;
    }

    public async Task<SlaPolicyPagedResultReadModel> GetPagedByCompanyIdAsync(Guid companyId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var baseQuery = _context.SlaPolicies.AsNoTracking().Where(x => x.CompanyId == companyId);
        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new SlaPolicyReadModel
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive,
                Rules = _context.SlaRules
                    .Where(r => r.SlaPolicyId == p.Id)
                    .Select(r => new SlaRuleReadModel { Id = r.Id, FirstResponseMinutes = r.FirstResponseMinutes, ResolutionMinutes = r.ResolutionMinutes, Name = r.Name })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new SlaPolicyPagedResultReadModel { Items = items, Total = total, Page = page, Size = pageSize };
    }

    public async Task SetActiveAsync(Guid policyId, Guid companyId, CancellationToken cancellationToken = default)
    {
        await _context.SlaPolicies
            .Where(x => x.CompanyId == companyId && x.IsActive == true)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsActive, false), cancellationToken);

        await _context.SlaPolicies
            .Where(x => x.Id == policyId && x.CompanyId == companyId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsActive, true), cancellationToken);
    }
}
