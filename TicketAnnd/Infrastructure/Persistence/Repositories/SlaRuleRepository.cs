using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class SlaRuleRepository : ISlaRuleRepository
{
    private readonly TicketAnndDbContext _context;

    public SlaRuleRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(SlaRule rule, CancellationToken cancellationToken = default)
    {
        _context.SlaRules.Add(rule);
        return Task.CompletedTask;
    }

    public async Task<SlaRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SlaRules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(SlaRule rule, CancellationToken cancellationToken = default)
    {
        _context.SlaRules.Update(rule);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SlaRule rule, CancellationToken cancellationToken = default)
    {
        _context.SlaRules.Remove(rule);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SlaRuleReadModel>> GetByPolicyIdAsync(Guid slaPolicyId, CancellationToken cancellationToken = default)
    {
        return await _context.SlaRules
            .AsNoTracking()
            .Where(x => x.SlaPolicyId == slaPolicyId)
            .OrderBy(x => x.FirstResponseMinutes)
            .Select(x => new SlaRuleReadModel { Id = x.Id, FirstResponseMinutes = x.FirstResponseMinutes, ResolutionMinutes = x.ResolutionMinutes , Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
