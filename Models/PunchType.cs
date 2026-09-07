using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class PunchType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>Código funcional de la marca: IN, OUT, BREAK_IN, BREAK_OUT.</summary>
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
