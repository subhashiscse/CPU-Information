namespace CPUTracking.Services
{
    public class Result
    {
        public int contestId { get; set; }
        public string contestName { get; set; }
        public string handle { get; set; }
        public int rank { get; set; }
        public int ratingUpdateTimeSeconds { get; set; }
        public int oldRating { get; set; }
        public int newRating { get; set; }
    }
    class CodeforcesContestInfo
    {
        public string status { get; set; }
        public List<Result> result { get; set; }
    }
}