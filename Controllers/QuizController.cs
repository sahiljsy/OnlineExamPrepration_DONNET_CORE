using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Controllers
{
    public class QuizController: Controller
    {
        public IActionResult AddQuiz()
        {
            return View();
        }

        public IActionResult ViewQuiz()
        {
            return View();
        }
        public IActionResult UpdateQuiz()
        {
            return View();
        }
    }
}
