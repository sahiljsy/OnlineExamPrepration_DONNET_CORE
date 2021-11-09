using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineExamPrepration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Controllers
{
    public class PaperController: Controller
    {
        private readonly IPaperRepository _paperRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public PaperController( IPaperRepository paperRepository, IHostingEnvironment hostingEnvironment)
        {
            _paperRepository = paperRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        [Route("Paper/AllPapers/{ex}")]
        public IActionResult AllPapers(String ex)
        {
            IEnumerable<Paper> model = null;
            if (ex == "JEE")
            {
                model = _paperRepository.GetJeePapers();
            }
            else if(ex == "NEET")
            {
                model = _paperRepository.GetNeetPapers();
            }
            else
            {
                model = _paperRepository.GetGatePapers();
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult AddPaper()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPaper(PaperViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Paper != null)
                {                    
                    String uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Papers");
                    string filePath = Path.Combine(uploadsFolder, model.Paper.FileName);
                    var stream = new FileStream(filePath, FileMode.Create);
                    model.Paper.CopyTo(stream);
                    stream.Close();
                    
                }
                Paper newPaper = new Paper
                {
                    Exam = (exam)model.Exam,
                    Year = model.Year,
                    PaperPath = model.Paper.FileName,
                };

                _paperRepository.AddPaper(newPaper);
                return RedirectToAction("Display", "Home", new { area = "" });
            }
            return View();
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(hostingEnvironment.WebRootPath, "Papers/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        public IActionResult DeletePaper()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeletePaper(int a)
        {

            String Exam = Request.Form["Exam"];
            int year = int.Parse(Request.Form["year"]);
            _paperRepository.DeletePaper(Exam, year);
            return RedirectToAction("Display", "Home", new { area = "" });       
        }
    }
}
