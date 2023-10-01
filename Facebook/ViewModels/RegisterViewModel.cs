using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Facebook.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            DOB = DateTime.Now;
        }
        [Display(Name = "User Name")]
        public string UserName { get; set; }        
        public string Gender { get; set; }
        public string Address { get; set; }

        public IFormFile ProfImg { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Date Of Birth")]
        public DateTime DOB { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Password mismatch")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Mobile { get; set; }
        public string Profile { get; set; }
    }
}
