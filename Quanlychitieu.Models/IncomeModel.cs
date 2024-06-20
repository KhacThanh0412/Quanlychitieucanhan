using Newtonsoft.Json;

namespace Quanlychitieu.Models;

public class IncomeModel
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    public DateTime DateReceived { get; set; }
    public double AmountReceived { get; set; }
    public string? Reason { get; set; }
    public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDateTime { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public string UserId { get; set; }
}
