using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using CPUTracking.Models.Generic;

namespace CPUTracking.Models.Create
{
	public class Contest : GenericPropertyForCRUD
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ContestStartTime { get; set; }
        public DateTime? ContestEndTime { get; set; }
        public TimeSpan? ContestDuration { get; set; }
        public int? TotalParticipant { get; set; }
    }
}

