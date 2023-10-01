using Microsoft.AspNetCore.Mvc;
using Facebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Http;
using Facebook.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    public class AccountController : Controller
    {
        private readonly FbContext _fbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        private readonly IWebHostEnvironment _env;
        public AccountController( FbContext fbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)//, IHttpContextAccessor httpContextAccessor
        {
            _fbContext = fbContext;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _env = env;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if(!ModelState.IsValid)
            {
                return View(login);
            }
            User model= _fbContext.Users.Where(u => u.UserName == login.UserName && u.Password == login.Password).FirstOrDefault();
            if(model==null)
            {
                ModelState.AddModelError("", "Invalid credentils");
                return View(login);
            }
            _session.SetString("username", model.UserName);
            _session.SetInt32("userid", model.UserID);
            if(model.Profile==null)
            {
                _session.SetString("profile", "~/Img/prof.jpg");
            }
            else
                _session.SetString("profile", model.Profile);

            var postContent=_fbContext.Posts.Where(p => p.PostBy == model.UserID).Select(s => s.PostContent).ToList<string>();
            var keys = _fbContext.DataKeys.AsNoTracking().ToList();
            string Character= AnalyzeUserCharacter.GetCharacteristics(postContent, keys);
            model.Character = Character;
            _fbContext.Users.Update(model);
            _fbContext.SaveChanges();
            _session.SetString("character", Character);

            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            _session.Clear();            
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            User model = _fbContext.Users.Where(r => r.UserName == register.UserName).FirstOrDefault();
            if (model!=null)
            {
                ModelState.AddModelError("UserName", "User Name already exists.");
                return View(register);
            }
            string PrfileURl = "";
            if(register.ProfImg!=null)
            {
                string basePath = System.IO.Path.Combine(_env.WebRootPath, "ProfImg");
                PrfileURl= await FileHelper.UploadFile(basePath, 0, register.ProfImg, 1);
            }

            User user = new User
            {
                UserName = register.UserName,
                Password = register.Password,
                DOB = register.DOB,
                Gender = register.Gender,
                Address = register.Address,
                Mobile = register.Mobile,
                Profile= PrfileURl
            };
            _fbContext.Users.Add(user);
            int RecCnt=await _fbContext.SaveChangesAsync();
            if (RecCnt == 0)
            {
                ModelState.AddModelError("", "User registration failed");
                return View(register);
            }
            return RedirectToAction("Login");
            
        }

    }
}
