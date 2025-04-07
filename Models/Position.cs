using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmployeeAPI.Models
{
    public class Position
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string DepartmentId { get; set; }
    }
}
