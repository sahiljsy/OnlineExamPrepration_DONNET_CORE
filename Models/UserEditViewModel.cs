using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Models
{
    public class UserEditViewModel
    {
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Phone]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
        public string ExistingPath { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
