using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ISlaRuleRepository
{
    Task AddAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task<Entities.SlaRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlaRuleReadModel>> GetByPolicyIdAsync(Guid slaPolicyId, CancellationToken cancellationToken = default);
}
