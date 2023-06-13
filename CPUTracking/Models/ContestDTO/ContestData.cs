
using System;
using Newtonsoft.Json;

namespace CPUTracking.Models.ContestDTO
{
    //public class ContestData
    //{
    //       public string status { get; set; }
    //       public List<Result> result { get; set; }
    //   }
    //   public class Result
    //   {
    //       public int contestId { get; set; }
    //       public string contestName { get; set; }
    //       public string handle { get; set; }
    //       public int rank { get; set; }
    //       public int ratingUpdateTimeSeconds { get; set; }
    //       public int oldRating { get; set; }
    //       public int newRating { get; set; }
    //   }
    public class ContestData
    {
        public string Place { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public double Percentage { get; set; }
        public int Point { get; set; }
        public int? TotalParticipant { get; set; }
    }
    public class Contests
    {
        public string Place { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        //public string Kind { get; set; }
        //public int Resource { get; set; }
        //public double Score { get; set; }
        //public int Solved { get; set; }
        //public string Division { get; set; }
        //public int Cid { get; set; }
        //public int Sid { get; set; }
        //public int? RatingChange { get; set; }
        //public int NewRating { get; set; }
        //public int? OldRating { get; set; }
        //public ContestValues Values { get; set; }
        //public string Slug { get; set; }
        //public int NProblems { get; set; }
        //public string When { get; set; }
    }
    public class ContestValues
    {
        public int new_rating { get; set; }
        public double raw_rating { get; set; }
        public int n_solved { get; set; }
        public int place { get; set; }
        public int score { get; set; }
    }
    public class AtcoderJp
    {
        public int pk { get; set; }
        public object kind { get; set; }
        public string host { get; set; }
        public List<Color> colors { get; set; }
        public string icon { get; set; }
        public List<string> fields { get; set; }
        public List<List<Contests>> data { get; set; }
        public Highest highest { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class CodechefCom
    {
        public int pk { get; set; }
        public object kind { get; set; }
        public string host { get; set; }
        public List<Color> colors { get; set; }
        public string icon { get; set; }
        public List<string> fields { get; set; }
        public List<List<Contests>>  data { get; set; }
        public Highest highest { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class CodeforcesCom
    {
        public int pk { get; set; }
        public object kind { get; set; }
        public string host { get; set; }
        public List<Color> colors { get; set; }
        public string icon { get; set; }
        public List<string> fields { get; set; }
        public List<List<Contests>> data { get; set; }
        public Highest highest { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class Color
    {
        public List<double> hsl { get; set; }
        public int low { get; set; }
        public int high { get; set; }
        public string name { get; set; }
        public int next { get; set; }
        public int prev { get; set; }
        public string color { get; set; }
        public string hex_rgb { get; set; }
    }

    public class Data
    {
        public Resources resources { get; set; }
        public List<DateTime> dates { get; set; }
    }

    public class Highest
    {
        public int value { get; set; }
        public int timestamp { get; set; }
    }

    public class LeetcodeCom
    {
        public int pk { get; set; }
        public object kind { get; set; }
        public string host { get; set; }
        public List<Color> colors { get; set; }
        public string icon { get; set; }
        public List<string> fields { get; set; }
        public List<List<Contests>>  data { get; set; }
        public Highest highest { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class Resources
    {
        [JsonProperty("codeforces.com")]
        public CodeforcesCom codeforcescom { get; set; }

        //[JsonProperty("codechef.com")]
        //public CodechefCom codechefcom { get; set; }

        //[JsonProperty("atcoder.jp")]
        //public AtcoderJp atcoderjp { get; set; }

        //[JsonProperty("leetcode.com")]
        //public LeetcodeCom leetcodecom { get; set; }
    }

    public class Root
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
    public class ContestStandingsResponse
    {
        public string Status { get; set; }
        public StandingsResult Result { get; set; }
    }

    public class StandingsResult
    {
        public StandingsRow[] Rows { get; set; }
    }

    public class StandingsRow
    {
    }
}

