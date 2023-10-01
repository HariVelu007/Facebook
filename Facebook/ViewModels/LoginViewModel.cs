using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facebook.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name ="User Name")]
        [StringLength(50)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    
}
