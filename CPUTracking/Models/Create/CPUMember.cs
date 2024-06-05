using System.ComponentModel.DataAnnotations;
using CPUTracking.Models.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CPUTracking.Models.Create
{
    public class CPUMember : GenericPropertyForCRUD
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public String? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Please Enter a Valid Email")]
        public string? Email { get; set; } = string.Empty;
        public string? SessionId { get; set; } = string.Empty;
        public string? Session { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? CodeforcesId { get; set; } = string.Empty;
        public string? CodechefId { get; set; } = string.Empty;
        public string? AtCoderId { get; set; } = string.Empty;
        public string? LightOjId { get; set; } = string.Empty;
        public string? TophId { get; set; } = string.Empty;
        public string? ClistId { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}

