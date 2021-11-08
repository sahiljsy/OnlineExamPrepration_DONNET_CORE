using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public interface IQuizRepository
    {
        QuizViewModel Add(QuizViewModel quizViewModel);
        bool Delete(int id);
        QuizViewModel Get(int id);
        IEnumerable<QuizViewModel> GetAllJee(String Level);
        IEnumerable<QuizViewModel> GetAllNeet(String Level);
        IEnumerable<QuizViewModel> GetAllGate(String Level);
        QuizViewModel Edit(QuizViewModel quizViewModel);
    }
}
