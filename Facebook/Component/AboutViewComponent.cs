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
    public class AboutViewComponent : ViewComponent
    {
        private readonly FbContext _fbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public AboutViewComponent(IHttpContextAccessor httpContextAccessor, FbContext fbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _fbContext = fbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            TempData["profile"] = _session.GetString("profile") ?? "~/Img/prof.jpg";

            UserAbout user=await _fbContext.Users.Where(u => u.UserID == _session.GetInt32("userid"))
                .Select(s=>new UserAbout
                {
                    UserName=s.UserName,
                    Gender=s.Gender,
                    Age=(DateTime.Now.Subtract(s.DOB).Days)/365,
                    Character=s.Character==null?"Undefined":s.Character
                }).FirstOrDefaultAsync();

            return View(user);
        }
    }
}
