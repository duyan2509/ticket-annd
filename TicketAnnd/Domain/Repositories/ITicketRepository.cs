using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<TicketPagedResultReadModel> GetPagedByCompanyIdAsync(Guid companyId, int page = 1, int pageSize = 10, Guid? teamId = null, string? status = null, Guid? categoryId = null, string? subject = null, bool? unassigned = null, CancellationToken cancellationToken = default);
    Task<Ticket?> GetTrackingByIdCompanyIdAsync(Guid requestTicketId, Guid CompanyId, CancellationToken cancellationToken);
    Task<TicketReadModel?> GetByCompanyIdAndCodeAsync(Guid companyId, string ticketCode, CancellationToken cancellationToken = default);
    Task UpdateAsync(Ticket ticket);
}