using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TicketAnnd.Application.Common;

namespace TicketAnnd.Infrastructure.Services;

public class MailtrapEmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private ILogger<MailtrapEmailSender>? _logger;
    public MailtrapEmailSender(IConfiguration config, ILogger<MailtrapEmailSender>? logger = null)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string bodyHtml, CancellationToken cancellationToken = default)
    {
        var host = _config["Mailtrap:Host"] ?? "smtp.mailtrap.io";
        var port = _config.GetValue<int>("Mailtrap:Port", 2525);
        var user = _config["Mailtrap:UserName"];
        var password = _config["Mailtrap:Password"];
        var from = _config["Mailtrap:FromEmail"] ?? "noreply@ticketannd.com";

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            return;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = bodyHtml };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(user, password, cancellationToken);  
        await client.SendAsync(message, cancellationToken);
        _logger?.LogInformation("Email sent successfully.");
        await client.DisconnectAsync(true, cancellationToken);
    }
}
