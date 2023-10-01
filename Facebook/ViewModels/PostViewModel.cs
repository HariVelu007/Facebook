using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.ViewModels
{
    public class PostViewModel
    {        
        public string PostContent { get; set; }
        public IFormFile PostImg { get; set; }        
    }
    public class PostDetailViewModel
    {
        public int ID { get; set; }
        public string PostContent { get; set; }
        public string PostImg { get; set; }
        public string PostBy { get; set; }
        public string UserProf { get; set; }
        public DateTime PostOn { get; set; }
    }
    public class UserAbout
    {
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Character { get; set; }
    }
}
