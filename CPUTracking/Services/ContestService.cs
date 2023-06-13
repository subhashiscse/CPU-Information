using System;
using System.Configuration;
using CPUTracking.Models.Create;
using MongoDB.Driver;

namespace CPUTracking.Services
{
	public class ContestService :IContestService
	{
        private readonly IMongoCollection<Contest> _contestList;
        public ContestService(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _contestList = database.GetCollection<Contest>("Contests");
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
    }
}

