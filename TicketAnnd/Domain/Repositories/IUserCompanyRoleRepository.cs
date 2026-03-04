using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface IUserCompanyRoleRepository
{
    Task AddAsync(UserCompanyRole userCompanyRole, CancellationToken cancellationToken = default);
    Task<UserCompanyRoleReadModel?> GetFirstByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserCompanyRoleReadModel?> GetByUserIdAndCompanyIdAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);
    Task<CompanyPagedResultReadModel> GetCompanyOptionsPagedByUserIdAsync(Guid userId, int page  = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
