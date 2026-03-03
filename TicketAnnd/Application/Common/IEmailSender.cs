namespace TicketAnnd.Application.Common;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string bodyHtml, CancellationToken cancellationToken = default);
}
