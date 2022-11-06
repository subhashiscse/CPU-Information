using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPUTracking.Models.Create;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Member()
        {
            var data = new CPUMember();
            data.Name = "Subashis Mollick";
            data.Email = "smollickcseiu@gmail.com";

            return View();
        }
        private readonly IMongoCollection<CPUMember> _departments;
        public MemberInfoController(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("EmployeeAppCon"));
            var database = dbClient.GetDatabase("CPUTracking");
            _departments = database.GetCollection<CPUMember>("Members");
        }
        [HttpGet]
        public JsonResult GetAllData()
        {

            var data = _departments.AsQueryable();
            return new JsonResult(data);
        }
    }
}

