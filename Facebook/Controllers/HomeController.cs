using Facebook.Helpers;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Controllers
{
    public class HomeController : Controller
    {
        private readonly FbContext _fbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        public HomeController(FbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _fbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int UserID = _session.GetInt32("userid") ?? 0;
            int pagesize = 10;
            int pageno = 1;

            if (UserID == 0)
            {
                _session.Clear();
                return View("Login", "Account");
            }
           
            var frirndlist = _fbContext.Friends.Where(f => f.UserID == _session.GetInt32("userid"))
                    .Select(s => s.FriendID).ToList();

            var model = (from p in _fbContext.Posts
                         join u in _fbContext.Users on p.PostBy equals u.UserID into up
                         from u in up.DefaultIfEmpty()
                         where frirndlist.Contains(p.PostBy) ||  p.PostBy == _session.GetInt32("userid")

                         select new PostDetailViewModel
                         {
                             ID = p.PostID,
                             PostBy = u.UserName,
                             PostContent = p.PostContent,
                             PostImg = p.PostImg,
                             PostOn = p.PostedOn,
                             UserProf = u.Profile
                         })
                    .AsNoTracking()
                    .OrderByDescending(o => o.PostOn)
                    .Skip(pagesize * (pageno - 1)).Take(pagesize);

            var t =model.ToQueryString();

            return View(model.ToList());
        }       

        [HttpPost]
        public async Task<IActionResult> PostStatus(PostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("PostStatus", viewModel);
            }
            int UserID = _session.GetInt32("userid") ?? 0;
            if (UserID == 0)
            {
                _session.Clear();
                return View("Login", "Account");
            }
            string fileFullName = "";
            if (viewModel.PostImg!=null && viewModel.PostImg.FileName!="")
            {
                string basePath = Path.Combine(_env.WebRootPath, "PostedImg", $"{UserID}");
                fileFullName= await FileHelper.UploadFile(basePath, UserID, viewModel.PostImg);
            }
                      
            Post post = new Post()
            {
                PostBy = UserID,
                PostContent = viewModel.PostContent,
                PostImg = fileFullName,
                PostedOn = DateTime.Now
            };
            _fbContext.Posts.Add(post);
            int status =await _fbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult LikePost()
        {
            return View("Index");
        }

        [HttpPost]
        public IActionResult DisLikePost()
        {
            return View("Index");
        }

    }
}
