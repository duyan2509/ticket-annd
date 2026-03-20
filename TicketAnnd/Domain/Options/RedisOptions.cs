namespace TicketAnnd.Domain.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string Connection { get; set; } = string.Empty;
}
