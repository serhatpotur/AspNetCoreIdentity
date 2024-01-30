using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleListViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
            return View(roles);
        }
        [HttpGet]
        public IActionResult RoleAdd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleAddViewModel model)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = model.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            return RedirectToAction(nameof(RolesController.Index));
        }

        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            RoleEditViewModel model = new() { Id = role.Id, Name = role.Name };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var role = await _roleManager.FindByIdAsync(model.Id);
            role.Name = model.Name;
            await _roleManager.UpdateAsync(role);

            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return View();
            }
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
            }
            return RedirectToAction(nameof(RolesController.Index));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRoleToUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.userId = userId;
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name
                };

                if (userRoles.Contains(role.Name)) // kullanıcıda bu rol var
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }
            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> modelList)
        {
            var user = await _userManager.FindByIdAsync(userId);
            foreach (var role in modelList)
            {
                if (role.Exist)
                    await _userManager.AddToRoleAsync(user, role.Name);

                else
                    await _userManager.RemoveFromRoleAsync(user, role.Name);

            }

            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
