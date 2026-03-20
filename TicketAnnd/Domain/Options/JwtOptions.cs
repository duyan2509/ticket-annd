namespace TicketAnnd.Domain.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TicketAnnd";
    public string Audience { get; set; } = "TicketAnnd";
    public int ExpiryMinutes { get; set; } = 60;
}
