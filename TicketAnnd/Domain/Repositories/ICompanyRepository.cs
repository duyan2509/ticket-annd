using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Domain.Repositories;

public interface ICompanyRepository
{
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
}
