using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CPUTracking.Models.Login
{
	public class UserLogin
	{
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public String? Id { get; set; }
        [Required(ErrorMessage = "User Name Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Please Enter a Valid Email")]
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
    }
}

