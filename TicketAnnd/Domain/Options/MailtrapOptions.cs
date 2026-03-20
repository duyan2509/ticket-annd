namespace TicketAnnd.Domain.Options;

public class MailtrapOptions
{
    public const string SectionName = "Mailtrap";

    public string Host { get; set; } = "smtp.mailtrap.io";
    public int Port { get; set; } = 2525;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@ticketannd.com";
}
