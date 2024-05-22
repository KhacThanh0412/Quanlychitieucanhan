using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Quanlychitieu.Models;

public class IDsToBeDeleted
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string UserID { get; set; }
    public string PlatformModel { get; set; }
}
