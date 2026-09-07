using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = null!;

        /// <summary>Identificador IANA de zona horaria, por ejemplo "America/Santiago".</summary>
        [Required]
        [StringLength(64)]
        public string Timezone { get; set; } = null!;
    }
}
