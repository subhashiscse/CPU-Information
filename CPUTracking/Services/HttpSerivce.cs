using System;
using System.Net.Http;
using CPUTracking.Models.Create;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CPUTracking.Services
{
	public class HttpSerivce
	{
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://api.example.com/";

        public HttpSerivce(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<object> GetContestInfo()
        {
            //HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}todos");
            //response.EnsureSuccessStatusCode();

            //string responseBody = await response.Content.ReadAsStringAsync();
            //var todoItems = JsonConvert.DeserializeObject<IEnumerable<object>>(responseBody);
            //return todoItems;
            string CodeforcesHandle = "Subhashis_CSE";
            string url = $"https://codeforces.com/api/user.rating?handle={CodeforcesHandle}";
            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(url);
            string jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();

            var ContestList = JsonConvert.DeserializeObject<CodeforcesContestInfo>(jsonResponse);
            return ContestList;
        }
    }
}

