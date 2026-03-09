using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ISlaPolicyRepository
{
    Task AddAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task<Entities.SlaPolicy?> GetPolicyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task<SlaPolicyPagedResultReadModel> GetPagedByCompanyIdAsync(Guid companyId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task SetActiveAsync(Guid policyId, Guid companyId, CancellationToken cancellationToken = default);
    Task<SlaRuleReadModel?> GetReadRuleByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task<Entities.SlaRule?> GetRuleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.SlaRule rule, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlaRuleReadModel>> GetRulesByPolicyIdAsync(Guid slaPolicyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlaRuleReadModel>> GetActiveRulesAsync(Guid companyId, CancellationToken cancellationToken);
}
