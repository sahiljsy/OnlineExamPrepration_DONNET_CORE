using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class PaperViewModel
    {
        public int Id { get; set; }
        public enum exam
        {
            JEE,
            NEET,
            GATE
        }
        [Required]
        public exam Exam { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public IFormFile Paper { get; set; }
    }
}
