using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmplyeeManagements.Models;
using EmplyeeManagements.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmplyeeManagements.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

     //   [AcceptVerbs("Get","Post")]
        [HttpGet] [HttpPost]
        public async Task<IActionResult> IsEmailInUse( string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return  Json(true);
            }
            else
            {
                return Json($"Email {email} is already taken");
            }
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
       
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if(_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                       
                        return RedirectToAction("UsersList", "Administration");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
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
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
          return  View();

        }

       
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(UserLogInViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
                    model.RememberMe, false);
                if (result.Succeeded)
                {
                    if(! string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
              
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                
            }

            return View(model);
        }

        
    }
}