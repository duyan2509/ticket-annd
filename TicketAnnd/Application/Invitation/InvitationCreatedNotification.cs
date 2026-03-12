using MediatR;
using TicketAnnd.Application.Common;
using Microsoft.Extensions.Configuration;
using Hangfire;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Application.Invitation;

public class InvitationCreatedNotificationHandler : INotificationHandler<InvitationCreatedNotification>
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _config;

    public InvitationCreatedNotificationHandler(IEmailSender emailSender, IConfiguration config)
    {
        _emailSender = emailSender;
        _config = config;
    }

    public async Task Handle(InvitationCreatedNotification notification, CancellationToken cancellationToken)
    {
        var companyName = notification.CompanyName ?? "your company";
        var subject = "You're invited to join " + companyName;

        var fe_base_url = _config["FE_URL"] ?? string.Empty;

        var registerLink = string.IsNullOrWhiteSpace(fe_base_url)
            ? string.Empty
            : $"{fe_base_url}/register?email={Uri.EscapeDataString(notification.To)}";

        var body = $@"<p>You have been invited to join <strong>{companyName}</strong> as <strong>{notification.Role}</strong>.</p>
<p>Please create an account using this email address ({notification.To}), then sign in to accept or reject the invitation.</p>
{(string.IsNullOrWhiteSpace(registerLink) ? string.Empty : $"<p><a href=\"{registerLink}\">Create account</a></p>")}
<p>This invitation expires in {notification.ExpireDays} days.</p>";

        BackgroundJob.Enqueue<IEmailSender>(x =>
            x.SendAsync(notification.To, subject, body, CancellationToken.None)
        );
    }

}
