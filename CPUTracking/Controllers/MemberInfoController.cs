using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPUTracking.Models.Create;
using CPUTracking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class MemberInfoController : Controller
    {
        private readonly IMongoCollection<CPUMember> _memberList;
        public ISessionService sessionService;
        public MemberInfoController(IConfiguration configuration, ISessionService sessionService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _memberList = database.GetCollection<CPUMember>("Members");
            this.sessionService = sessionService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MemberList()
        {
            var data = _memberList.Find(FilterDefinition<CPUMember>.Empty).SortBy(c => c.CreateDate).ToList();
            return View(data);
        }
        public IActionResult CreateMember()
        {
            var sessionList = this.sessionService.GetAllSessionList();
            return View();
        }
        [HttpPost]
        public IActionResult CreateMember(CPUMember member)
        {
            member.Id = Guid.NewGuid().ToString();
            _memberList.InsertOne(member);
            ViewBag.Mgs = "Member Added SuccessFully";
            return RedirectToAction("MemberList");
        }

        public IActionResult EditMember(string Id)
        {
            //var o_id = new ObjectId(Id);
            var member = _memberList.Find(c => c.Id == Id).FirstOrDefault();
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }
        [HttpPost]
        public ActionResult EditMember(CPUMember member)
        {
            if (string.IsNullOrEmpty(member?.Id.ToString()))
            {
                ViewBag.Mgs = "Please provide id";
                return View(member);
            }
            _memberList.ReplaceOne(c => c.Id == member.Id, member);
            return RedirectToAction("MemberList");
        }

        public ActionResult DeleteMember(string id)
        {
            var member= _memberList.Find(c => c.Id == id).FirstOrDefault();
            if (member == null)
            {
                ViewBag.Msg = "Please provide Id";
                return NotFound();
            }
            return View(member);
        }

        [HttpPost]
        public ActionResult DeleteMember(CPUMember member)
        {
            _memberList.DeleteOne(c => c.Id == member.Id);
            return RedirectToAction("MemberList");
        }

    }
}

