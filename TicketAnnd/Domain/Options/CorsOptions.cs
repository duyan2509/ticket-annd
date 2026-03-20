namespace TicketAnnd.Domain.Options;

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = new[]
    {
        "http://localhost:3000",
        "http://localhost:3001",
        "http://localhost:5173"
    };
}
