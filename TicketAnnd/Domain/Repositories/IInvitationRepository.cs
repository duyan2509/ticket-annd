using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvitationReadModel>> GetByEmailAsync(string email, InviationStatuses? status = null, string? companyName = null, int page = 1, int size = 50, CancellationToken cancellationToken = default);
    Task<bool> PendingExists(string email, Guid companyId);
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default);
    Task<CompanyInvitationPagedResultReadModel> GetByCompanyIdAsync(Guid companyId, int page = 1, int size = 50, CancellationToken cancellationToken = default);
    Task<Invitation> GetByEmailAndCompanyIdAsync(string normalizedEmail, Guid companyId, CancellationToken cancellationToken);
}
