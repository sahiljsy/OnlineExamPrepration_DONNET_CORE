using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineExamPrepration.Models;
using OnlineExamPrepration.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamPrepration.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> userManager;
        private readonly SignInManager<UserModel> signInManager;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger<AccountController> logger;
        private readonly IMailService mailService;
        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager,
            IWebHostEnvironment hostingEnvironment, ILogger<AccountController> logger, IMailService mailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.mailService = mailService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
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
                    DOB = model.DOB,
                    Profile_pic = PicName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var ConfirmEmailLink = Url.Action("COnfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, ConfirmEmailLink);
                    await userManager.AddToRoleAsync(user, "User");
                    try
                    {
                        MailRequest mail = new MailRequest
                        {
                            ToEmail = user.Email,
                            Subject = "onfirmEmail your Mial",
                            Body = @"Hello,\n TO Confirm Email Please Click \n < a href ='" + ConfirmEmailLink + "'>Here</a>"
                        };
                        await mailService.SendEmailAsync(mail);
                        ViewBag.ErrorTitle = "Registration SuccessFul";
                        ViewBag.ErrorMessage = "Before Login Please Confirm Your Email We have sent Confirmation link to your Registerd Email";
                        return View("Error");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    
                    //await signInManager.SignInAsync(user, isPersistent: true);
                    //UserModel userId = await userManager.GetUserAsync(HttpContext.User);
                    
                    //return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);  
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = "User not Found!";
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "EMail Can't be Confirmed!";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Display", "Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null && !user.EmailConfirmed &&
                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email is Not Confirmed Yet!");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Display", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login");

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword","Account",
                        new { email = model.Email, token = token }, Request.Scheme);
                    try
                    {
                        MailRequest mail = new MailRequest
                        {
                            ToEmail = user.Email,
                            Subject = "onfirmEmail your Mial",
                            Body = @"Hello,\n TO Reset Passwordd Please clicK\n< a href ='" + passwordResetLink + "'>Here</a>"
                        };
                        await mailService.SendEmailAsync(mail);
                        return View("ForgotPasswordConfirmation");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid Attempt");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);

                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);

        }

        public async Task<IActionResult> Display()
        {
            var userId = await userManager.GetUserAsync(HttpContext.User);
            return View(userId);      
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var userId = await userManager.GetUserAsync(HttpContext.User);
            UserEditViewModel user = new UserEditViewModel
            {
                Id = userId.Id,
                Email = userId.Email,
                UserName = userId.UserName,
                ExistingPath = userId.Profile_pic,
                PhoneNumber = userId.PhoneNumber
            };
            return View(user);
        }
        [HttpPost]

        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await userManager.GetUserAsync(HttpContext.User);
                userId.UserName = model.UserName;
                userId.PhoneNumber = model.PhoneNumber;
                string PicName = null;
                if (model.ProfilePicture != null)
                {
                    if(model.ExistingPath != null)
                    {
                        string filepath = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePic", model.ExistingPath);
                        System.IO.File.Delete(filepath);
                    }
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePic");
                    PicName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                    string filPath = Path.Combine(uploadFolder, PicName);
                    model.ProfilePicture.CopyTo(new FileStream(filPath, FileMode.Create));
                    userId.Profile_pic = PicName;
                }
                var result = await userManager.UpdateAsync(userId);
                if (result.Succeeded)
                {
                    return RedirectToAction("Display", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var userId = await userManager.GetUserAsync(HttpContext.User);
            UserEditViewModel user = new UserEditViewModel
            {
                Id = userId.Id,
                Email = userId.Email,
                UserName = userId.UserName,
                ExistingPath = userId.Profile_pic,
                PhoneNumber = userId.PhoneNumber
            };
            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm()
        {
            try
            {
                var userId = await userManager.GetUserAsync(HttpContext.User);
                await signInManager.SignOutAsync();
                if (userId.Profile_pic != null)
                {
                    string filepath = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePic", userId.Profile_pic);
                    System.IO.File.Delete(filepath);
                }
                await userManager.DeleteAsync(userId);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult DisplayAll()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        [ActionName("DisplayAll")]

        public IActionResult DisplayAllConforim()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data  
                var users = userManager.Users.Select(m => new { m.Id, m.UserName, m.PhoneNumber, m.DOB, m.Email });

                //Sorting  
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    Users = Users.OrderBy(sortColumn + " " + sortColumnDirection);
                //}
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    users = users.Where(m => m.Id == searchValue ||
                    m.UserName == searchValue ||
                    m.Email == searchValue ||
                    m.DOB.ToString() == searchValue || 
                    m.PhoneNumber == searchValue);
                    
                }

                //total number of rows count   
                recordsTotal = users.Count();
                //Paging   
                var data = users.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
