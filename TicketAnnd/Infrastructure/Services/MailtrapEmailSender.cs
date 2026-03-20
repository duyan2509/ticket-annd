using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Options;

namespace TicketAnnd.Infrastructure.Services;

public class MailtrapEmailSender : IEmailSender
{
    private readonly MailtrapOptions _mailtrapOptions;
    private readonly ILogger<MailtrapEmailSender>? _logger;

    public MailtrapEmailSender(IOptions<MailtrapOptions> mailtrapOptions, ILogger<MailtrapEmailSender>? logger = null)
    {
        _mailtrapOptions = mailtrapOptions.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string bodyHtml, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_mailtrapOptions.UserName) || string.IsNullOrEmpty(_mailtrapOptions.Password))
            return;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_mailtrapOptions.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = bodyHtml };

        using var client = new SmtpClient();
        await client.ConnectAsync(_mailtrapOptions.Host, _mailtrapOptions.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_mailtrapOptions.UserName, _mailtrapOptions.Password, cancellationToken);  
        await client.SendAsync(message, cancellationToken);
        _logger?.LogInformation("Email sent successfully.");
        await client.DisconnectAsync(true, cancellationToken);
    }
}
