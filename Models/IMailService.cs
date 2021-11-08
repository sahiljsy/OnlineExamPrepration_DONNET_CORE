using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
