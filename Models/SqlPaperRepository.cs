using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class SqlPaperRepository : IPaperRepository
    {
        private readonly AppDbContext context;
        public SqlPaperRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Paper AddPaper(Paper paper)
        {
            context.Papers.Add(paper);
            context.SaveChanges();
            return paper;
        }

        public void DeletePaper(string Exam, int year)
        {
            exam ex;
            if(Exam == "0")
            {
                ex = exam.JEE;
            }
            else if(Exam == "1")
            {
                ex = exam.NEET;
            }
            else
            {
                ex = exam.GATE;
            }
            IEnumerable<Paper> paper = from item in context.Papers where item.Year == year && item.Exam == ex select item;
            foreach(var item in paper)
            {
                context.Papers.Remove(item);
            }
            context.SaveChanges();
        }

        public IEnumerable<Paper> GetNeetPapers()
        {
            IEnumerable<Paper> list = from item in context.Papers where item.Exam == exam.NEET orderby item.Year descending select item;
            return list;
        }

        public IEnumerable<Paper> GetJeePapers()
        {
            IEnumerable<Paper> list = from item in context.Papers where item.Exam == exam.JEE orderby item.Year descending select item;
            return list;
        }

        public IEnumerable<Paper> GetGatePapers()
        {
            IEnumerable<Paper> list = from item in context.Papers where item.Exam == exam.GATE orderby item.Year descending select item;
            return list;
        }
    }
}
