using System.Text.Json.Serialization;

namespace TicketAnnd.Domain.ReadModels;

public class CompanyInvitationItemReadModel
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("response_at")]
    public DateTime? ResponseAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("expire_at")]
    public DateTime ExpireAt { get; set; }
}
