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
    public class LoginController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginUser()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

