using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public enum exam
    {
        JEE,
        NEET,
        GATE
    }
    public class QuizViewModel
    {
        public int Id { get; set; }
        [Required]
        public exam Exam { get; set; }
        [Required]
        public String Level { get; set; }
        [Required]
        public String Question { get; set; }
        [Required]
        public String OptionA { get; set; }
        [Required]
        public String OptionB { get; set; }
        [Required]
        public String OptionC { get; set; }
        [Required]
        public String OptionD { get; set; }
        [Required]
        public String Answer { get; set; }
        [Required]
        public String Solution { get; set; }

    }
}
