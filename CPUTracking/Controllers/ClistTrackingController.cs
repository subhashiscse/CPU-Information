﻿using System;
using System.Globalization;
using CPUTracking.Models.Create;
using CPUTracking.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CPUTracking.Controllers
{
	public class ClistTrackingController: Controller
    {
        private readonly IMongoCollection<ClistRank> _contestList;
        private readonly IMongoCollection<CPUMember> _memberList;
        public IContestService contestService;
        public ClistTrackingController(IConfiguration configuration, IContestService contestService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<ClistRank>("ClistRanks");
            _memberList = database.GetCollection<CPUMember>("Members");
            this.contestService = contestService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> ContestList(string HandleName, DateTime FromDate)
        {
            if (FromDate == DateTime.MinValue)
            {
                FromDate = DateTime.Today.AddMonths(-1);
            }
            List<ClistRank> contestList = GetContestList(HandleName, FromDate);
            ViewBag.StartingMessage = "Contest list from " + FromDate.ToString("dd-MM-yyyy") + " to " + DateTime.Now.ToString("dd-MM-yyyy");
            ViewBag.CoderName = "Id Name: " + HandleName;
            return View(contestList);
        }
        public List<ClistRank> GetContestList(string HandleName, DateTime FromDate)
        {
            if (FromDate == DateTime.MinValue)
            {
                FromDate = DateTime.Today.AddMonths(-1);
            }
            List<ClistRank> contestList = _contestList.Find(c => c.ContestDate >= FromDate && c.UserName == HandleName).SortByDescending(c => c.ContestDate).ToList();
            return contestList;
        }
        public async Task<ActionResult> UpdateContestDataForAllUser()
        {
            var memberList = _memberList.Find(FilterDefinition<CPUMember>.Empty).SortBy(c => c.CreateDate).ToList();
            foreach(var member in memberList)
            {
                List<int> totalPage = new List<int>() { 1,2};
                string clistHandleName = member.ClistId;
                foreach(int pageno in totalPage)
                {
                    UpdateContestListData(pageno, clistHandleName);
                }
            }
            return RedirectToAction("ContestList");
        }
        public async void UpdateContestListData(int PageNo,string HandleName)
        {
            HtmlWeb web = new HtmlWeb();
            if (HandleName == null)
            {
                HandleName = "Mohidul";
            }
            string profileLink = "https://clist.by/coder/"+ HandleName + "/?contest_page=1";
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
                                var result = _contestList.Find(c => c.ContestId == contestId && c.UserName==HandleName).FirstOrDefault();
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
                                    else {
                                        currentContestDate = DateTime.Parse(contestDate);
                                    }
                                    int percentage = int.Parse(rank)*100 / int.Parse(total);
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
                                    _contestList.InsertOne(currentContest);
                                }
                            }
                        }
                        index++;
                    }
                }
            }
            ViewBag.Msg = "The contest data for " + HandleName+ " Updated Successfully";
            ViewBag.CoderName = "Id Name: " + HandleName;
            return;
            //return RedirectToAction("ContestList");
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
    }
    
}

