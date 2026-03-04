namespace TicketAnnd.Domain.ReadModels;

public class CompanyCurrentReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
