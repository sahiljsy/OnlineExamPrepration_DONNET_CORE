using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExamPrepration.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineExamPrepration.Controllers
{
    public class QuizController: Controller
    {
        private readonly AppDbContext _context;
        private readonly IQuizRepository _sqlQuizRepository;
        

        public QuizController(IQuizRepository sqlQuizRepository,AppDbContext context)
        {
            _context = context;
            _sqlQuizRepository = sqlQuizRepository;
            
        }

        [HttpGet]
        public IActionResult AddQuiz()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditQuiz(String id)
        {
            int Id = int.Parse(id);
            QuizViewModel model = _sqlQuizRepository.Get(Id);
            if(model == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        public IActionResult JEE(String Level)
        {
            if (Level == null)
                Level = "Easy";
            IEnumerable < QuizViewModel > model = _sqlQuizRepository.GetAllJee(Level);
            return View(model);
        }

        public IActionResult NEET(String Level)
        {
            if (Level == null)
                Level = "Easy";
            IEnumerable<QuizViewModel> model = _sqlQuizRepository.GetAllNeet(Level);
            return View(model);
        }

        public IActionResult GATE(String Level)
        {
            if (Level == null)
                Level = "Easy";
            IEnumerable<QuizViewModel> model = _sqlQuizRepository.GetAllGate(Level);
            return View(model);
        }

        public IActionResult Validate()
        {  
            int count = 0;
            IEnumerable<QuizViewModel> model;
            String level = Request.Form["Level"];
            String exm = Request.Form["Exam"];
           if(exm == "JEE")
            {
                model = _sqlQuizRepository.GetAllJee(level);
            }
           else if(exm == "NEET")
            {
                model = _sqlQuizRepository.GetAllNeet(level);
            }
            else if(exm == "GATE")
            {
                model = _sqlQuizRepository.GetAllGate(level);
            }
            else
            {
                return View("Error");
            }
            
            foreach(var quiz in model)
            {
                var score = Request.Form[quiz.Id.ToString()];
                if (score == quiz.Answer)
                {
                   count++;
                }              
            }
            ViewBag.score = count;
            return View(model);
        }

        [HttpPost]
        public IActionResult EditQuiz(QuizViewModel quizViewModel)
        {
            if(ModelState.IsValid)
            {
                var quiz = _sqlQuizRepository.Edit(quizViewModel);
                return RedirectToAction("UpdateQuiz");
            }
            return View(quizViewModel);
        }

        public IActionResult DeleteQuiz(int id)
        {
            //int Id = int.Parse(id);
            _sqlQuizRepository.Delete(id);
            return RedirectToAction("UpdateQuiz");
        }

        [HttpPost]
        public IActionResult AddQuiz(QuizViewModel quiz)
        {
            if(ModelState.IsValid)
            {
                QuizViewModel newQuiz = _sqlQuizRepository.Add(quiz);
                return RedirectToAction("UpdateQuiz");
            }
            return View();
        }

        public IActionResult UpdateQuiz()
        {
            return View();
        }
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data  
                var quizdata = _context.QuizViewModels.Select(m => new { m.Id,m.Exam,m.Level, m.Question, m.OptionA, m.OptionB, m.OptionC, m.OptionD, m.Answer, m.Solution });
                //var quizdata = _context.QuizViewModels;
                ////Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    quizdata = quizdata.Where(q => q.Level == searchValue || q.Question == searchValue || q.OptionA == searchValue || q.OptionB == searchValue || q.OptionC == searchValue || q.OptionD == searchValue || q.Answer ==searchValue || q.Solution == searchValue);
                   // customerData = customerData.Where(m => m.Name == searchValue);
                }

                ////total number of rows count   
                recordsTotal = quizdata.Count();
                ////Paging   
                var data = quizdata.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
               // 
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
