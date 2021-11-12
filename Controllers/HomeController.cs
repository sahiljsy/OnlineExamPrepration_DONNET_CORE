using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineExamPrepration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
           
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        [Authorize]
        public IActionResult Display()
        {
            return View();
        }

        
    }
}
