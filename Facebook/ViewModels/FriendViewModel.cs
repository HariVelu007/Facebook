using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.ViewModels
{
    public class FriendViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Profile { get; set; }
        public string Status { get; set; }

      
    }

    public class FriendSrchViewModel
    {       
        public string SearchName{ get; set; }
        public string SearchType { get; set; }
    }

    public class FriendFullViewModel
    {
        public FriendSrchViewModel SrchViewModel { get; set; }
        public List<FriendViewModel> friendViewModels { get; set; }
    }
}
