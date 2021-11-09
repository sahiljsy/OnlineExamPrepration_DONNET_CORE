using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public interface IPaperRepository
    {
        Paper AddPaper(Paper paper);
        void DeletePaper(String Exam, int year);
        IEnumerable<Paper> GetNeetPapers();
        IEnumerable<Paper> GetGatePapers();
        IEnumerable<Paper> GetJeePapers();
    }
}
