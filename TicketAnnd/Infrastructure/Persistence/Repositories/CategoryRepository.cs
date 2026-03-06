using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly TicketAnndDbContext _context;

    public CategoryRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Categrory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        return Task.CompletedTask;
    }

    public async Task<Categrory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Categrory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Categrory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<CategoryReadModel>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryReadModel { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);
    }
}
