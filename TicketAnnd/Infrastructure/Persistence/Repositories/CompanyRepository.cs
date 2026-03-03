using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;

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
}
