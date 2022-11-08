using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CPUTracking.Models.Create
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public String Id { get; set; }
        public string? Name { get; set; }
    }
}
