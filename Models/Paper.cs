using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class Paper
    {
        
        public int Id { get; set; }

        [Required]
        public exam Exam { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public String PaperPath { get; set; }


    }
}
