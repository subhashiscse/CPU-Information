using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using CPUTracking.Models.Generic;

namespace CPUTracking.Models.Create
{
    public class Session : GenericPropertyForCRUD
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public String Id { get; set; }
        public string? Name { get; set; }
    }
}
