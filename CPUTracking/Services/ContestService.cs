using System;
using System.Configuration;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;
using MongoDB.Driver;

namespace CPUTracking.Services
{
	public class ContestService :IContestService
	{
        private readonly IMongoCollection<Contest> _contestList;
        private readonly IMongoCollection<CPUMember> _memberList;
        private readonly IMongoCollection<ClistRank> _clistContestList;
        public ContestService(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<Contest>("Contests");
            _clistContestList = database.GetCollection<ClistRank>("ClistRanks");
            _memberList = database.GetCollection<CPUMember>("Members");
        }

        public int CalculatePointUsingScore(int percentage)
        {
            if (percentage <= 20)
            {
                return 5;
            }
            else if (percentage <= 40)
            {
                return 4;
            }
            else if (percentage <= 60)
            {
                return 3;
            }
            else if (percentage <= 80)
            {
                return 2;
            }
            return 1;
        }

        public List<Contest> GetAllContestList()
        {
            try
            {
                List<Contest> data = _contestList.Find(FilterDefinition<Contest>.Empty).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        List<ContestScore> IContestService.GenerateScoreForAllUser(DateTime FromDate)
        {
            if (FromDate == DateTime.MinValue)
            {
                FromDate = DateTime.Today.AddMonths(-1);
            }
            var memberList = _memberList.Find(FilterDefinition<CPUMember>.Empty).SortBy(c => c.Session).ToList();
            List<ContestScore> ContestScoreList = new List<ContestScore>();
            foreach (var member in memberList)
            {
                string clistHandleName = member.ClistId;
                List<ClistRank> contestList = _clistContestList.Find(c => c.ContestDate >= FromDate && c.UserName == clistHandleName).SortByDescending(c => c.ContestDate).ToList();
                ContestScore contestScore = new ContestScore();
                contestScore.Name = member.Name;
                contestScore.HandleName = clistHandleName;
                contestScore.Session = member.Session;
                contestScore.NumberOfContest = contestList.Count();
                contestScore.TotalScore = contestList.Sum(obj => obj.Point);
                ContestScoreList.Add(contestScore);
            }
            return ContestScoreList;
        }

        public string checkContestPlatformName(string inputString)
        {
            if (inputString.Contains("codeforces"))
            {
                return "codeforces";
            }
            else if (inputString.Contains("codechef"))
            {
                return "codechef";
            }
            else if (inputString.Contains("atcoder"))
            {
                return "atcoder";
            }
            else
            {
                return "other";
            }
        }
    }
}

