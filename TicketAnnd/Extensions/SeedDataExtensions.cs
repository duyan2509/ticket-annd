using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Options;
using TicketAnnd.Infrastructure.Persistence;

namespace TicketAnnd.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedSuperAdminAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TicketAnndDbContext>();
        var seedAdminOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;

        if (string.IsNullOrWhiteSpace(seedAdminOptions.Email) || string.IsNullOrWhiteSpace(seedAdminOptions.Password))
            return;

        var exists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == seedAdminOptions.Email, cancellationToken);

        if (exists)
            return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = seedAdminOptions.Email.Trim(),
            PasswordHash = PasswordHasher.Hash(seedAdminOptions.Password),
            IsActive = true,
            IsSuperAdmin = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}
