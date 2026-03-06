using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Categrory category, CancellationToken cancellationToken = default);
    Task<Categrory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Categrory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Categrory category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryReadModel>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}
