using Microsoft.EntityFrameworkCore;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Infrastructure.Persistence;

namespace TicketAnnd.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedSuperAdminAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TicketAnndDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var exists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (exists)
            return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            PasswordHash = PasswordHasher.Hash(password),
            IsActive = true,
            IsSuperAdmin = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}
