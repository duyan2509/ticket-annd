using Microsoft.EntityFrameworkCore;

namespace TicketAnnd.Persistence;

public class TicketAnndDbContext : DbContext
{
    public TicketAnndDbContext(DbContextOptions<TicketAnndDbContext> options) : base(options)
    {
    }

   
}

