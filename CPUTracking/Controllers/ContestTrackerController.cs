using System;
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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class ContestTrackerController : Controller
    {
        private readonly IMongoCollection<Contest> _contestList;
        public IContestService _contestService;
        public ContestTrackerController(IConfiguration configuration, IContestService contestService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<Contest>("Contests");
            _contestService = contestService;
        }
    public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ContestList(string idurl)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://clist.by/coder/sojol/ratings/";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    //Console.Write(json);
                    Root reponse = JsonConvert.DeserializeObject<Root>(json);

                    List<ContestData> contestList = await prepareCodeforcesDataAsync(reponse.data);
                    return View(contestList);
                }
            }
            return View();
        }
        public async Task<List<ContestData>> prepareCodeforcesDataAsync(Data clistData)
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
                    ContestData currentContestData = new ContestData();
                    int contestId = int.Parse(data.Key);
                    Contest contest =  _contestList.Find(c => c.Id == contestId).FirstOrDefault();
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
                        break;
                    }
                }
            }
            return codeforceContestFinalData;

        }

        //List<Contest> prepareLeetcodeData(Data clistData)
        //{
        //    Resources resourcesData = clistData.resources;
        //    List<Contest> leecodeContestFinalData = new List<Contest>(); 
        //    if (clistData != null)
        //    {
        //        resourcesData = clistData.resources;
        //    }
        //    if (resourcesData == null)
        //    {
        //        Console.WriteLine("Resources data is null");
        //    }
        //    else
        //    {
        //        var leetcodeData = resourcesData.leetcodecom;
        //        var leetcodeContestData = leetcodeData.data[0];
        //        leecodeContestFinalData = leetcodeContestData;
        //        /*foreach (var data in leetcodeContestData)
        //        {
        //            Console.WriteLine(data.Place);
        //            Console.WriteLine(data.Date);
        //        }*/
        //    }
        //    return leecodeContestFinalData;
        //}
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

