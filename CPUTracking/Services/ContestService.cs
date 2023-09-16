using System;
using System.Configuration;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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
        public async Task SyncContestDataForAParticularUser(string handleName)
        {
            List<int> totalPage = new List<int>() { 1, 2 };
            string clistHandleName = handleName;
            foreach (int pageno in totalPage)
            {
                UpdateContestListData(pageno, clistHandleName);
            }
        }
        public void UpdateContestListData(int PageNo, string HandleName)
        {
            HtmlWeb web = new HtmlWeb();
            if (HandleName == null)
            {
                HandleName = "Cloud_";
            }
            string profileLink = "https://clist.by/coder/" + HandleName + "/?contest_page=" + PageNo;
            HtmlDocument doc = web.Load(profileLink);
            var table = doc.DocumentNode.SelectSingleNode("//*[@id=\"contests\"]");

            if (table != null)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows != null)
                {
                    var index = 0;
                    foreach (var row in rows)
                    {
                        if (row != null && index > 0)
                        {
                            var rankProgressDiv = row.SelectSingleNode(".//div[contains(@class, 'rank-progress')]");

                            if (rankProgressDiv != null)
                            {

                                HtmlNode contestLinktd = row.SelectSingleNode("td[8]");
                                HtmlNode dateTd = row.SelectSingleNode("td[6]");

                                HtmlNode contestLinkHrefLinkTag = contestLinktd.SelectSingleNode("a[1]");
                                var html = rankProgressDiv.OuterHtml;
                                var data = html.Split("<br>", StringSplitOptions.RemoveEmptyEntries);
                                string rank = "";
                                string total = "";
                                string rowHtml = row.OuterHtml;
                                HtmlDocument rowDoc = new HtmlDocument();
                                rowDoc.LoadHtml(rowHtml);
                                HtmlNode trNode = rowDoc.DocumentNode.SelectSingleNode("//tr[@class='contest']");
                                string contestId = trNode.GetAttributeValue("id", "");
                                var result = _clistContestList.Find(c => c.ContestId == contestId && c.UserName == HandleName).FirstOrDefault();
                                if (result == null)
                                {
                                    foreach (string item in data)
                                    {
                                        if (item.Contains("Rank:"))
                                        {
                                            rank = item.Replace("Rank:", "").Trim();
                                        }
                                        else if (item.Contains("Total:"))
                                        {
                                            var totalDiv = item.Split("\"", StringSplitOptions.RemoveEmptyEntries);
                                            total = totalDiv[0].Replace("Total:", "").Trim();
                                        }
                                    }
                                    string contestDate = dateTd.InnerText;
                                    var contestName = contestLinkHrefLinkTag.InnerText;
                                    string contestLink = contestLinkHrefLinkTag.GetAttributeValue("href", "");
                                    bool containsSubstring = contestDate.Contains("Sept");
                                    DateTime currentContestDate;
                                    if (containsSubstring)
                                    {
                                        contestDate = contestDate.Replace("Sept.", "Sep.");
                                        currentContestDate = DateTime.Parse(contestDate);
                                    }
                                    else if (contestDate == "today")
                                    {
                                        currentContestDate = DateTime.Today;
                                    }
                                    else if (contestDate == "yesterday")
                                    {
                                        currentContestDate = DateTime.Today.AddDays(-1);
                                    }
                                    else
                                    {
                                        currentContestDate = DateTime.Parse(contestDate);
                                    }
                                    int percentage = int.Parse(rank) * 100 / int.Parse(total);
                                    ClistRank currentContest = new ClistRank();
                                    currentContest.Id = Guid.NewGuid().ToString();
                                    currentContest.ContestId = contestId;
                                    currentContest.ContestName = contestName;
                                    currentContest.ContestLink = contestLink;
                                    currentContest.ContestDate = currentContestDate;
                                    currentContest.Rank = int.Parse(rank);
                                    currentContest.TotalParticipant = int.Parse(total);
                                    currentContest.Percentage = percentage;
                                    currentContest.Point = CalculatePointUsingScore(percentage);
                                    currentContest.ContestPlatform = checkContestPlatformName(contestLink);
                                    currentContest.UserName = HandleName;
                                    _clistContestList.InsertOne(currentContest);
                                }
                            }
                        }
                        index++;
                    }
                }
            }
        }
    }
}

