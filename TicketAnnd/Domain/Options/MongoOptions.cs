namespace TicketAnnd.Domain.Options;

public class MongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
}
