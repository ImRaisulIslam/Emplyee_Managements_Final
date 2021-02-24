using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmplyeeManagements.Models;
using EmplyeeManagements.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmplyeeManagements.Controllers
{
  // [Authorize(Policy = "ManageAdministration")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager
            , UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this.logger = logger;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }



        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("ErrorPageShowMessage");
            }

            var existingUserClaim = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };

                if(existingUserClaim.Any(x=>x.Type == claim.Type && x.Value == "True"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }



        [HttpPost]
       [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaim(UserClaimViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("ErrorPageShowMessage");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user , claims);

            if( !result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            //result = await _userManager.AddClaimsAsync(user,
            //   model.Claims.Where(c => c.IsSelected).Select(c =>
            //   new Claim(c.ClaimType, c.ClaimType)));



            result = await _userManager.AddClaimsAsync(user,
               model.Claims.Select(c =>
               new Claim(c.ClaimType, c.IsSelected ? "True": "False")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }


        [HttpGet]
        public IActionResult UsersList()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpPost]
      
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("ErrorPageShowMessage");
            }
            else
            {
                try
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("UsersList");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("UsersList");

                }
                catch (DbUpdateException ex)
                {

                    logger.LogError($"Exception Occured : {ex}");
                    ViewBag.ErrorTitle = $"{user.UserName} Is In Role ";
                    ViewBag.ErrorMessage = $"{user.UserName} User cannot be deleted as there are roles or Claims to this user. " +
                        $"If you want to delete this user, please remove the role and claims from the user and then try to delete";
                    return View("Error");
                }

               



            }
        }


        [HttpPost]


        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("ErrorPageShowMessage");
            }
            else
            {


                try
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleList");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("RoleList");

                }
                catch (DbUpdateException ex)

                {
                    logger.LogError($"Exception Occured : {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("Error");

                }
            }
        }












        [HttpGet]


        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id :{id} Not Found";
                return View("ErrorPageShowMessage");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),

            };

            return View(model);
        }







        [HttpPost]
        
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }



        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }


       


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.Name,
                };
                IdentityResult identityResult = await _roleManager.CreateAsync(identityRole);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        public IActionResult RoleList()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
       
        
        [HttpGet]               
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id :{id} Not Found";
                return View("ErrorPageShowMessage");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,


            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
       
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id :{model.Id} Not Found";
                return View("ErrorPageShowMessage");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id :{roleId} Not Found";
                return View("ErrorPageShowMessage");
            }

            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,

                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("ErrorPageShowMessage");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });

        }



        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRole(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {UserId} cannot be found";
                return View("ErrorPageShowMessage");
            }

            else
            {
                var model = new List<ManageUserRoleViewModel>();
                foreach (var role in _roleManager.Roles)
                {
                    var manageUserRoleViewModel = new ManageUserRoleViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        manageUserRoleViewModel.IsSelected = true;
                    }

                    else
                    {
                        manageUserRoleViewModel.IsSelected = false;
                    }

                    model.Add(manageUserRoleViewModel);

                }



                return View(model);

            }


        }


        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRole(List<ManageUserRoleViewModel> model, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {UserId} cannot be found";
                return View("ErrorPageShowMessage");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if( !result.Succeeded)
            {
                ModelState.AddModelError("", "Can Not Remove Existing Roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can Not Remove Existing Roles");
                return View(model);
            }

            return RedirectToAction("EditUser",  new {id = UserId });

        }

    }
}