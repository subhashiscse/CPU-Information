using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using CPUTracking.Models.ContestDTO;
using CPUTracking.Models.Create;
using CPUTracking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using OfficeOpenXml;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CPUTracking.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class MemberInfoController : Controller
    {
        private readonly IMongoCollection<CPUMember> _memberList;
        public ISessionService sessionService;
        public IContestService _contestService;
        public MemberInfoController(IConfiguration configuration,
            ISessionService sessionService,
            IContestService contestService)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("CPUTrackingAppConnection"));
            var database = dbClient.GetDatabase("CPUTracking");
            _memberList = database.GetCollection<CPUMember>("Members");
            this.sessionService = sessionService;
            this._contestService = contestService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MemberList()
        {
            var data = _memberList.Find(FilterDefinition<CPUMember>.Empty).SortBy(c => c.Session).ToList();
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
        public ActionResult MemberDetails(string Id)
        {
            var member = _memberList.Find(c => c.Id == Id).FirstOrDefault();
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }
        public ActionResult SyncContestData(string Id)
        {
            var member = _memberList.Find(c => c.Id == Id).FirstOrDefault();
            this._contestService.SyncContestDataForAParticularUser(member.ClistId);
            return RedirectToAction("MemberList");
        }
        //Id Name Email Session PhoneNumber ClistId
        [HttpPost]
        public ActionResult UploadExcel(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet

                        // Get the total number of rows and columns
                        int rowCount = worksheet.Dimension.Rows;
                        int columnCount = worksheet.Dimension.Columns;
                        //List<string> headers = new List<string>();
                        //for(int col = 1; col <= columnCount; col++)
                        //{
                        //    var cellValue = worksheet.Cells[1, col].Value;
                        //    headers.Add(cellValue.ToString());
                        //}
                        //Console.WriteLine(headers);
                        for (int row = 2; row <= rowCount; row++)
                        {
                            CPUMember member = new CPUMember();
                            member.Id = Guid.NewGuid().ToString();
                            member.Name = (worksheet.Cells[row, 1]?.Value).ToString();
                            member.Email = (worksheet.Cells[row, 2]?.Value).ToString();
                            member.Session = (worksheet.Cells[row, 3]?.Value).ToString();
                            member.PhoneNumber = (worksheet.Cells[row, 4]?.Value).ToString();
                            member.ClistId = (worksheet.Cells[row, 5]?.Value).ToString();
                            _memberList.InsertOne(member);
                            Console.WriteLine(member);
                        }
                    }

                    return RedirectToAction("MemberList");
                }
                else
                {
                    ModelState.AddModelError("", "Please upload a valid Excel file.");
                }
            }

            return RedirectToAction("MemberList");
        }
    }
}

