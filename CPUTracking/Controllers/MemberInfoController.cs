using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPUTracking.Models.Create;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Driver;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class MemberInfoController : Controller
    {
        private readonly IMongoCollection<CPUMember> _memberList;
        public MemberInfoController(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _memberList = database.GetCollection<CPUMember>("Members");
        }
        public IActionResult Index()
        {
            //var data = _memberList.Find(FilterDefinition<CPUMember>.Empty).ToList();
            return View();
        }
        public IActionResult Member()
        {
            var data = _memberList.Find(FilterDefinition<CPUMember>.Empty).ToList();
            return View(data);
        }
        public IActionResult EditMember(string Id)
        {
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
            if (string.IsNullOrEmpty(member.Id))
            {
                ViewBag.Mgs = "Please provide id";
                return View(member);
            }
            _memberList.ReplaceOne(c => c.Id == member.Id, member);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteMember(string Id)
        {
            var member = _memberList.Find(c => c.Id == Id).FirstOrDefault();
            if (member == null)
            {
                return NotFound();
            }
            return View(member);

        }
    }
}

