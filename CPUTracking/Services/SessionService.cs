using CPUTracking.Models.Create;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CPUTracking.Services
{
    public class SessionService : ISessionService
    {
        private readonly IMongoCollection<Session> _sessionList;
        public SessionService(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _sessionList = database.GetCollection<Session>("Sessions");
        }
        public List<Session> GetAllSessionList()
        {
            try
            {
                List<Session> data = _sessionList.Find(FilterDefinition<Session>.Empty).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
