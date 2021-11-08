using Microsoft.AspNetCore.Identity;
using OnlineExamPrepration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.ViewModel
{
    public class AdminCreateUserViewModel: RegisterViewModel
    {
        public AdminCreateUserViewModel()
        {
        }
       public List<string> RoleName { get; set; }
    }
}
