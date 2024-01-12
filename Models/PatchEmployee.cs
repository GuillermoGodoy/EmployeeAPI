using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Reflection;

namespace EmployeeAPI.Models
{
    public class PatchEmployee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(this));
            }

            return dictionary;
        }
    }
}
