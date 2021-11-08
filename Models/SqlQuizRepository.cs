using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class SqlQuizRepository: IQuizRepository
    {
        private readonly AppDbContext context;

        public SqlQuizRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<QuizViewModel> GetAllJee(String Level)
        {
            IEnumerable<QuizViewModel> list = from item in context.QuizViewModels where item.Exam == exam.JEE && item.Level == Level select item;
            return list;
        }
        public IEnumerable<QuizViewModel> GetAllNeet(String Level)
        {
            IEnumerable<QuizViewModel> list = from item in context.QuizViewModels where item.Level == Level && item.Exam == exam.NEET select item;
            return list;
        }
        public IEnumerable<QuizViewModel> GetAllGate(String Level)
        {
            IEnumerable<QuizViewModel> list = from item in context.QuizViewModels where item.Level == Level && item.Exam == exam.GATE select item;
            return list;
        }
        public QuizViewModel Add(QuizViewModel quizViewModel)
        {
            context.QuizViewModels.Add(quizViewModel);
            context.SaveChanges();
            return quizViewModel;
        }
        public bool Delete(int Id)
        {
            QuizViewModel q = context.QuizViewModels.Find(Id);
            if (q != null)
            {
                context.QuizViewModels.Remove(q);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }    
        }

        public QuizViewModel Get(int Id)
        {
            return context.QuizViewModels.Find(Id);
        }

        public QuizViewModel Edit(QuizViewModel quizViewModel)
        {
            var quiz = context.QuizViewModels.Attach(quizViewModel);
            quiz.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return quizViewModel;
        }
    }
}
