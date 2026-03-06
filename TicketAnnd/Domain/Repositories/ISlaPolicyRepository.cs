using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ISlaPolicyRepository
{
    Task AddAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task<Entities.SlaPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.SlaPolicy policy, CancellationToken cancellationToken = default);
    Task<SlaPolicyPagedResultReadModel> GetPagedByCompanyIdAsync(Guid companyId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task SetActiveAsync(Guid policyId, Guid companyId, CancellationToken cancellationToken = default);
}
