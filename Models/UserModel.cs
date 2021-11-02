using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class UserModel: IdentityUser
    {
        public string Profile_pic { get; set; }
        public DateTime DOB { get; set; }

    }
}
