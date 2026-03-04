using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Domain.Repositories;

public interface ICompanyRepository
{
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
    Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<TicketAnnd.Domain.ReadModels.CompanyCurrentReadModel?> GetReadModelByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}
