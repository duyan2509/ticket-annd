using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly TicketAnndDbContext _context;

    public CompanyRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Add(company);
        return Task.CompletedTask;
    }

    public async Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);
    }

    public async Task<TicketAnnd.Domain.ReadModels.CompanyCurrentReadModel?> GetReadModelByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Id == companyId)
            .Select(c => new TicketAnnd.Domain.ReadModels.CompanyCurrentReadModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug
            })
            .FirstOrDefaultAsync(c => c.Id != Guid.Empty, cancellationToken);
    }

  
}
