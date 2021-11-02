using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineExamPrepration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> userManager;
        private readonly SignInManager<UserModel> signInManager;
        private readonly IWebHostEnvironment hostingEnvironment;
        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string PicName = null;
                if(model.ProfilePicture != null)
                {
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePic");
                    PicName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                    string filPath = Path.Combine(uploadFolder, PicName);
                    model.ProfilePicture.CopyTo(new FileStream(filPath, FileMode.Create));
                }
                var user = new UserModel
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    DOB = (DateTime)model.DOB,
                    Profile_pic = PicName
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);  
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login");

            }
            return View(model);
        }
    }
}
