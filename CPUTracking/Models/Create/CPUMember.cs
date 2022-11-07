using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CPUTracking.Models.Create
{
    public class CPUMember
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public String Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Session { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CodeforcesId { get; set; }
        public string? CodechefId { get; set; }
        public string? AtCoderId { get; set; }
        public string? LightOjId { get; set; }
        public string? TophId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

