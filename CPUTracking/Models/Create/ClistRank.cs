using System;
using CPUTracking.Models.Generic;

namespace CPUTracking.Models.Create
{
	public class ClistRank: GenericPropertyForCRUD
	{
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string UserName { get; set; }
        public int Rank { get; set; }
        public int TotalParticipant { get; set; }
        public int Percentage { get; set; }
        public int Point { get; set; }
        public string? ContestName { get; set; }
        public DateTime? ContestDate { get; set; }
        public string ContestLink { get; set; }
        public string ContestPlatform { get; set; }
    }
}

