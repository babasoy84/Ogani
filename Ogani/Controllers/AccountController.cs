using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogani.Models.Concretes;
using Ogani.ViewModels;

namespace Ogani.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Username
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, true);

                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    return RedirectToAction("Index", "Shop");
                }
                else
                {
                    foreach (var item in result.Errors)
                        ModelState.AddModelError(item.Code, item.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    //if (await userManager.IsEmailConfirmedAsync(user))
                    //{
                    //    var result = await signInManager.PasswordSignInAsync(user, model.Password, model.IsPersistent, true);

                    //    if (result.Succeeded)
                    //    {
                    //        if (!string.IsNullOrWhiteSpace(returnUrl))
                    //            return Redirect(returnUrl);
                    //        return RedirectToAction("Index", "Shop");
                    //    }
                    //    else if (result.IsLockedOut) ModelState.AddModelError("login", "Lockout");
                    //    else ModelState.AddModelError("login", "Incorrect username or password");
                    //}
                    //else ModelState.AddModelError("login", "Confirm Email");


                    if (await userManager.CheckPasswordAsync(user, model.Password))
                    {
                        await signInManager.SignInAsync(user, model.IsPersistent);
                        if (!string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Index", "Shop");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Shop");
        }
    }
}
