using CPUTracking.Models.Create;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CPUTracking.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IMongoCollection<Session> _sessionList;
        public SessionController(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _sessionList = database.GetCollection<Session>("Sessions");
        }
        public IActionResult SessionList()
        {
            var data = _sessionList.Find(FilterDefinition<Session>.Empty).ToList();
            return View(data);
        }
        public IActionResult CreateSession()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateSession(Session session)
        {
            try
            {
                session.Id = Guid.NewGuid().ToString();
                _sessionList.InsertOne(session);
                ViewBag.Mgs = "Session Added SuccessFully";
                return RedirectToAction("SessionList");
            }
            catch (Exception ex)
            {
                ViewBag.Mgs = "Session Already Exists";
                throw;
            }
        }
        public IActionResult EditSession(string Id)
        {
            //var o_id = new ObjectId(Id);
            var session = _sessionList.Find(c => c.Id == Id).FirstOrDefault();
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }
        [HttpPost]
        public ActionResult EditSession(Session session)
        {
            if (string.IsNullOrEmpty(session.Id.ToString()))
            {
                ViewBag.Mgs = "Please provide id";
                return View(session);
            }
            _sessionList.ReplaceOne(c => c.Id == session.Id, session);
            return RedirectToAction("SessionList");
        }

        public ActionResult DeleteSession(string id)
        {
            var session = _sessionList.Find(c => c.Id == id).FirstOrDefault();
            if (session == null)
            {
                ViewBag.Msg = "Please provide Id";
                return NotFound();
            }
            return View(session);
        }

        [HttpPost]
        public ActionResult DeleteSession(Session session)
        {
            _sessionList.DeleteOne(c => c.Id == session.Id);
            return RedirectToAction("SessionList");
        }
    }
}
