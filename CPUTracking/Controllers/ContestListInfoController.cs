using System;
using System.Collections.Generic;
using System.Linq;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;
using CPUTracking.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace CPUTracking.Controllers
{
    public class ContestListInfoController : Controller
    {
        private readonly IMongoCollection<Contest> _contestList;
        public IContestService contestService;
        public ContestListInfoController(IConfiguration configuration, IContestService contestService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<Contest>("Contests");
            this.contestService = contestService;
        }
        public async Task<ActionResult> ContestList(DateTime FromDate, DateTime ToDate)
        { 
            List<Contest> contestList = _contestList.Find(c => c.ContestStartTime >= FromDate && c.ContestStartTime <=ToDate).SortByDescending(c => c.ContestStartTime).ToList();
            return View(contestList);
        }
        public async Task<IActionResult> UpdateContestData(int contestId)
        {
            var contestData = _contestList.Find(c => c.Id == contestId).FirstOrDefault();
            if (contestData == null)
            {
                Console.WriteLine("Contest Id Not Found");
            } else
            {
                int participant = await GetTotalContestantData(contestId);
                contestData.TotalParticipant = participant;
                _contestList.ReplaceOne(c => c.Id == contestId, contestData); 
            }
            return RedirectToAction("ContestList");
        }
        public async Task<IActionResult> UpdateContestListData()
        {
            string apiUrl = "https://codeforces.com/api/contest.list";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ContestListResponse contestList = JsonConvert.DeserializeObject<ContestListResponse>(json);
                    List<Contest> codeforceContestList = new List<Contest>();
                    foreach (var contest in contestList.Result)
                    {
                        if (contest.Phase == "FINISHED")
                        {
                            var result = _contestList.Find(c => c.Id == contest.Id).FirstOrDefault();
                            if (result == null)
                            {
                                Contest currentContest = new Contest();
                                currentContest.Id = contest.Id;
                                currentContest.Name = contest.Name;
                                DateTime startTime = DateTimeOffset.FromUnixTimeSeconds(contest.StartTimeSeconds + 21600).DateTime;
                                TimeSpan duration = TimeSpan.FromSeconds(contest.DurationSeconds);
                                int numberofContestant = 0;
                                currentContest.ContestStartTime = startTime;
                                currentContest.ContestDuration = duration;
                                currentContest.TotalParticipant = numberofContestant;
                                _contestList.InsertOne(currentContest);
                            }
                            
                        }
                    }
                    ViewBag.Mgs = "Contest Added SuccessFully";
                    return RedirectToAction("ContestList");
                }
                else
                {
                    return Content("Error: " + response.StatusCode);
                }
            }
        }
        public async Task<int> GetTotalContestantData(long contestId)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://codeforces.com/api/contest.standings?contestId={contestId}&showUnofficial=false";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ContestStandingsResponse data = JsonConvert.DeserializeObject<ContestStandingsResponse>(jsonResponse);

                    int participantCount = data.Result.Rows.Length;
                    Console.WriteLine($"The total number of participants in the contest {contestId} is: {participantCount}");
                    return participantCount;
                }
                else
                {
                    return 0;
                }

            }
        }
        public async Task<IActionResult> CodeforcesDetails(string handleName)
        {
            handleName = "Cloud_";
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://codeforces.com/api/user.rating?handle={handleName}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    IndividualCodeforcesContestListResponse data = JsonConvert.DeserializeObject<IndividualCodeforcesContestListResponse>(jsonResponse);
                    List<CodeforcesContestList> codeforcesContestList = data.Result;
                    codeforcesContestList = codeforcesContestList.OrderByDescending(item => item.RatingUpdateTimeSeconds).ToList();
                    return View(codeforcesContestList);
                }

            }
            return View();
        }
        public async Task<IActionResult> AtcoderDetails(string handleName)
        {
            handleName = "Cloud_";
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://kenkoooo.com/atcoder/atcoder-api/results?user={handleName}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    IndividualAtcoderContestListResponse data = JsonConvert.DeserializeObject<IndividualAtcoderContestListResponse>(jsonResponse);
                    List<AtcoderContestList> codeforcesContestList = data.Result;
                    //codeforcesContestList = codeforcesContestList.OrderByDescending(item => item.RatingUpdateTimeSeconds).ToList();
                    return View(codeforcesContestList);
                }

            }
            return View();
        }
        public static async Task<int> GetAtcoderTotalParticipants(string contestId)
        {
            string url = $"https://atcoder.jp/contests/{contestId}/standings/json";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    //var contestData = await response.Content.ReadAsJsonAsync<AtcoderContestData>();
                    int totalParticipants = 0;// contestData.TotalCount;
                    return totalParticipants;
                }
            }

            // Return a default value if participant count cannot be retrieved
            return -1;
        }

    }
}

