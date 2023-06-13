using System;
namespace CPUTracking.Models.ContestDTO
{
	public class CFContestResponse
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public long StartTimeSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public string Phase { get; set; }
    }
    public class ContestListResponse
    {
        public string Status { get; set; }
        public List<CFContestResponse> Result { get; set; }
    }
    public class CFContest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalParticipant { get; set; }
    }
}

