using LiteDB;

namespace Quanlychitieu.Models;

public class IDsToBeDeleted
{
    [BsonId]
    public string? Id { get; set; }
    public string UserID { get; set; }
    public string PlatformModel { get; set; }
}
