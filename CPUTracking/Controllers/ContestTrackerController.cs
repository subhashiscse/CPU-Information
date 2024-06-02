using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Threading.Tasks;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;
using CPUTracking.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class ContestTrackerController : Controller
    {
        private readonly RestClient _client;
        private const string BaseUrl = "https://codeforces.com/api/";
        private readonly IMongoCollection<Contest> _contestList;
        public IContestService _contestService;
        public ContestTrackerController(IConfiguration configuration, IContestService contestService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<Contest>("Contests");
            _contestService = contestService;
            _client = new RestClient(BaseUrl);
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PersonalContestDetails(DateTime FromDate)
        {
            List<Contest> contestList = _contestList.Find(c => c.ContestStartTime >= FromDate).SortBy(c => c.CreateDate).ToList();
            return View(contestList);
        }
        public async Task<IActionResult> ContestList([FromRoute] string id, DateTime FromDate, DateTime ToDate)
        {
            using (HttpClient client = new HttpClient())
            {
                if (FromDate == DateTime.MinValue)
                {
                    FromDate = DateTime.Today.AddMonths(-1);
                }
                string url = "https://clist.by/coder/"+ id + "/ratings/";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    //Console.Write(json);
                    Root reponse = JsonConvert.DeserializeObject<Root>(json);
                    List<ContestData> codeforcesContestList = await prepareCodeforcesDataAsync(reponse.data, FromDate, ToDate);
                    //List<ContestData> codechefContestList = await prepareCodechefDataAsync(reponse.data, FromDate);
                    List<ContestData> atcoderContestList = await prepareAtcoderDataAsync(reponse.data, FromDate, ToDate);
                    List<ContestData> contestList = new List<ContestData>();
                    contestList = contestList.Concat(codeforcesContestList).ToList();
                    //contestList = contestList.Concat(codechefContestList).ToList();
                    contestList = contestList.Concat(atcoderContestList).ToList();
                    contestList = contestList.OrderByDescending(item => item.Date).ToList();
                    ViewBag.StartingMessage = "Contest list from "+FromDate.ToString("dd-MM-yyyy")+ " to "+DateTime.Now.ToString("dd-MM-yyyy");
                    ViewBag.CoderName = "Id Name: "+id;
                    return View(contestList);
                }
            }
            return View();
        }
        public async Task<List<ContestData>> prepareCodeforcesDataAsync(Data clistData,DateTime FromDate, DateTime ToDate)
        {
            Resources resourcesData = clistData.resources;
            List<ContestData> codeforceContestFinalData = new List<ContestData>();
            if (clistData != null)
            {
                resourcesData = clistData.resources;
            }
            if (resourcesData == null)
            {
                Console.WriteLine("Resources data is null");
            }
            else
            {
                var codeforcesData = resourcesData.codeforcescom;
                var codeforcesContestData = codeforcesData.data[0];
                var reversedData = codeforcesContestData.OrderByDescending(item => item.Date).ToList();
                int iterationCount = 0;
                foreach (var data in reversedData)
                {
                    if (data.Date >= FromDate)
                    {
                        ContestData currentContestData = new ContestData();
                        int contestId = int.Parse(data.Key);
                        Contest contest = _contestList.Find(c => c.Id == contestId).FirstOrDefault();
                        int numberofContestant = (int)contest.TotalParticipant;
                        double percentage = long.Parse(data.Place) * 100.0 / (numberofContestant);
                        currentContestData.Key = data.Key;
                        currentContestData.Date = data.Date;
                        currentContestData.Place = data.Place;
                        currentContestData.Name = data.Name;
                        currentContestData.Percentage = Math.Round(percentage, 0);
                        currentContestData.TotalParticipant = numberofContestant;
                        currentContestData.Point = CalculatePointUsingScore(percentage);
                        codeforceContestFinalData.Add(currentContestData);
                        iterationCount++;
                        if (iterationCount >= 10)
                        {
                            //break;
                        }
                    }
                }
            }
            return codeforceContestFinalData;

        }

        public async Task<List<ContestData>> prepareCodechefDataAsync(Data clistData, DateTime FromDate, DateTime ToDate)
        {
            Resources resourcesData = clistData.resources;
            List<ContestData> codechefContestFinalData = new List<ContestData>();
            if (clistData != null)
            {
                resourcesData = clistData.resources;
            }
            if (resourcesData == null)
            {
                Console.WriteLine("Resources data is null");
            }
            else
            {
                var codechefData = resourcesData.codechefcom;
                var codechefContestData = codechefData.data[0];
                var reversedData = codechefContestData.OrderByDescending(item => item.Date).ToList();
                int iterationCount = 0;
                foreach (var data in reversedData)
                {
                    if (data.Date >= FromDate)
                    {
                        ContestData currentContestData = new ContestData();
                        //int contestId = int.Parse(data.Key);
                        //Contest contest = _contestList.Find(c => c.Id == contestId).FirstOrDefault();
                        int numberofContestant = 10;
                        double percentage = long.Parse(data.Place) * 100.0 / (numberofContestant);
                        currentContestData.Key = data.Key;
                        currentContestData.Date = data.Date;
                        currentContestData.Place = data.Place;
                        currentContestData.Name = data.Name;
                        currentContestData.Percentage = Math.Round(percentage, 0);
                        currentContestData.TotalParticipant = numberofContestant;
                        currentContestData.Point = CalculatePointUsingScore(percentage);
                        codechefContestFinalData.Add(currentContestData);
                        iterationCount++;
                        if (iterationCount >= 10)
                        {
                            break;
                        }
                    }
                }
            }
            return codechefContestFinalData;

        }

        public async Task<List<ContestData>> prepareAtcoderDataAsync(Data clistData, DateTime FromDate, DateTime ToDate)
        {
            Resources resourcesData = clistData.resources;
            List<ContestData> atcoderContestFinalData = new List<ContestData>();
            if (clistData != null)
            {
                resourcesData = clistData.resources;
            }
            if (resourcesData == null)
            {
                Console.WriteLine("Resources data is null");
            }
            else
            {
                var atcoderData = resourcesData.atcoderjp;
                var atcoderContestData = atcoderData.data[0];
                var reversedData = atcoderContestData.OrderByDescending(item => item.Date).ToList();
                int iterationCount = 0;
                foreach (var data in reversedData)
                {
                    if (data.Date >= FromDate)
                    {
                        ContestData currentContestData = new ContestData();
                        //int contestId = int.Parse(data.Key);
                        //Contest contest = _contestList.Find(c => c.Id == contestId).FirstOrDefault();
                        int numberofContestant = 10;
                        double percentage = long.Parse(data.Place) * 100.0 / (numberofContestant);
                        currentContestData.Key = data.Key;
                        currentContestData.Date = data.Date;
                        currentContestData.Place = data.Place;
                        currentContestData.Name = data.Name;
                        currentContestData.Percentage = Math.Round(percentage, 0);
                        currentContestData.TotalParticipant = numberofContestant;
                        currentContestData.Point = CalculatePointUsingScore(percentage);
                        atcoderContestFinalData.Add(currentContestData);
                        iterationCount++;
                        if (iterationCount >= 10)
                        {
                            break;
                        }
                    }
                }
            }
            return atcoderContestFinalData;

        }

        public int CalculatePointUsingScore(double percentage)
        {
            if (percentage <= 20)
            {
                return 5;
            }
            else if(percentage <= 40)
            {
                return 4;
            }
            else if(percentage <= 60)
            {
                return 3;
            }
            else if(percentage <= 80)
            {
                return 2;
            }
            return 1;
        }

    }
}

