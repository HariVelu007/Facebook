using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Controllers
{
    public class FriendsController : Controller
    {
        private readonly FbContext _fbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public FriendsController(FbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _fbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _env = env;
        }
        [HttpGet]
        public IActionResult Index()
        {
            FriendFullViewModel model = new FriendFullViewModel();
            var UserFriendList = from f in _fbContext.Friends
                                 join u in _fbContext.Users on f.FriendID equals u.UserID into uf
                                 from u in uf.DefaultIfEmpty()
                                 where f.UserID == _session.GetInt32("userid") && f.Status == "Friend"
                                 select new FriendViewModel
                                 {
                                     UserID = f.FriendID,
                                     UserName = u.UserName,
                                     Profile = u.Profile,
                                     Status = f.Status
                                 };
            model.friendViewModels = UserFriendList.ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(FriendSrchViewModel viewModel)
        {
            FriendFullViewModel model = new FriendFullViewModel();
            IQueryable<FriendViewModel> viewModels = null;
            if (viewModel.SearchType == "Current Friends")//person who is friend to me
            {
                viewModels = from f in _fbContext.Friends
                             join u in _fbContext.Users on f.FriendID equals u.UserID into uf
                             from u in uf.DefaultIfEmpty()
                             where f.UserID == _session.GetInt32("userid") && f.Status == "Friend" 
                             && (viewModel.SearchName==null || (viewModel.SearchName!=null && u.UserName.Contains(viewModel.SearchName)))
                             select new FriendViewModel
                             {
                                 UserID = f.FriendID,
                                 UserName = u.UserName,
                                 Profile = u.Profile,
                                 Status = f.Status
                             };
            }
            else if (viewModel.SearchType == "New Friends")
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

                viewModels = from u in _fbContext.Users
                             where !NotIncludedList.Contains(u.UserID) && u.UserID != _session.GetInt32("userid")
                             && (viewModel.SearchName == null || (viewModel.SearchName != null && u.UserName.Contains(viewModel.SearchName)))
                             select new FriendViewModel
                             {
                                 UserID = u.UserID,
                                 UserName = u.UserName,
                                 Profile = u.Profile,
                                 Status = ""
                             };
            }
            else if (viewModel.SearchType == "Followers")//person who follows me
            {
                viewModels = from f in _fbContext.Friends
                             join u in _fbContext.Users on f.UserID equals u.UserID into uf
                             from u in uf.DefaultIfEmpty()
                             where f.FriendID == _session.GetInt32("userid") && f.Status == "Followed"
                             && (viewModel.SearchName == null || (viewModel.SearchName != null && u.UserName.Contains(viewModel.SearchName)))
                             select new FriendViewModel
                             {
                                 UserID = f.UserID,
                                 UserName = u.UserName,
                                 Profile = u.Profile,
                                 Status = "Followers"
                             };
            }
            else if (viewModel.SearchType == "Followed")//person i followed
            {
                viewModels = from f in _fbContext.Friends
                             join u in _fbContext.Users on f.FriendID equals u.UserID into uf
                             from u in uf.DefaultIfEmpty()
                             where f.UserID == _session.GetInt32("userid") && f.Status == "Followed"
                             && (viewModel.SearchName == null || (viewModel.SearchName != null && u.UserName.Contains(viewModel.SearchName)))
                             select new FriendViewModel
                             {
                                 UserID = f.FriendID,
                                 UserName = u.UserName,
                                 Profile = u.Profile,
                                 Status = f.Status
                             };
            }
            model.SrchViewModel = viewModel;
            model.friendViewModels = viewModels.ToList();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> RequestFriend(int ID)
        {
            Friend friend = new Friend
            {
                UserID = _session.GetInt32("userid") ?? 0,
                FriendID = ID,
                Status = "Followed"
            };
            _fbContext.Friends.Add(friend);

            await _fbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmFriend(int ID)
        {
            Friend friend = _fbContext.Friends.Where(f => f.FriendID == _session.GetInt32("userid") && f.UserID == ID).FirstOrDefault();
            friend.Status = "Friend";
            _fbContext.Friends.Update(friend);

            Friend NewFriend = new Friend
            {
                UserID = _session.GetInt32("userid") ?? 0,
                FriendID = ID,
                Status = "Friend"
            };
            _fbContext.Friends.Add(NewFriend);

            await _fbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> UnFriend(int ID)
        {
            Friend friend = _fbContext.Friends.Where(f => f.UserID == _session.GetInt32("userid") && f.FriendID == ID).FirstOrDefault();
            if (friend != null)
            {
                _fbContext.Friends.Remove(friend);
            }


            Friend updfriend = _fbContext.Friends.Where(f => f.FriendID == _session.GetInt32("userid") && f.UserID == ID).FirstOrDefault();
            if (updfriend != null)
            {
                updfriend.Status = "Followed";
                _fbContext.Friends.Update(updfriend); 
            }

            await _fbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> UnFollowFriend(int ID)
        {
            Friend friend = _fbContext.Friends.Where(f => f.UserID == _session.GetInt32("userid") && f.FriendID == ID).FirstOrDefault();
            if (friend != null)
            {
                _fbContext.Friends.Remove(friend);
                await _fbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeclineFriend(int ID)
        {
            Friend friend = _fbContext.Friends.Where(f => f.FriendID == _session.GetInt32("userid") && f.UserID == ID).FirstOrDefault();
            if (friend != null)
            {
                _fbContext.Friends.Remove(friend);
                await _fbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ViewFriend(int ID)
        {
            User user =await _fbContext.Users.Where(f => f.UserID == ID).FirstOrDefaultAsync();
            return PartialView("FriendDetail", user);
        }

    }
}
