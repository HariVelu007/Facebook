using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Component
{
    public class FriendSuggetionViewComponent: ViewComponent
    {
        private readonly FbContext _fbContext;  
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        public FriendSuggetionViewComponent(FbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _fbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var NotIncludedList1 = _fbContext.Friends.
                    Where(f => f.UserID == _session.GetInt32("userid"))
                    .Select(s => s.FriendID).ToList();
            var NotIncludedList2 = _fbContext.Friends.
                Where(f => f.FriendID == _session.GetInt32("userid"))
                .Select(s => s.UserID).ToList();
            var NotIncludedList = new List<int>();
            NotIncludedList.AddRange(NotIncludedList1);
            NotIncludedList.AddRange(NotIncludedList2);
            NotIncludedList.Remove(_session.GetInt32("userid") ?? 0);

            List<FriendViewModel> viewModels =await (from u in _fbContext.Users
                                                      where !NotIncludedList.Contains(u.UserID) && u.UserID != _session.GetInt32("userid") && u.Character==_session.GetString("character")
                                                      select new FriendViewModel
                                                      {
                                                          UserID = u.UserID,
                                                          UserName = u.UserName,
                                                          Profile = u.Profile,
                                                          Status = ""
                                                      }).Take(10).ToListAsync();
            return View(viewModels);
        }
    }
}
