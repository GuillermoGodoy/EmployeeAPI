using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    /// <summary>Credencial con la que un empleado se identifica en un dispositivo (PIN).</summary>
    public class Enrollment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Employee_Id { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Device_Id { get; set; }

        [Required]
        [RegularExpression("^[0-9]{4,10}$", ErrorMessage = "El PIN debe contener entre 4 y 10 dígitos.")]
        public string Pin { get; set; } = null!;

        public bool Active { get; set; } = true;
    }
}
