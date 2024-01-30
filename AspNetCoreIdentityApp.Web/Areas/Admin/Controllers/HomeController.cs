using AspNetCoreIdentityApp.DataAccess.Models;
using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var result = await _userManager.Users.ToListAsync();
            var viewModel = result.Select(x => new UserListViewModel
            {
                Email = x.Email,
                Id = x.Id,
                UserName = x.UserName
            });
            return View(viewModel);
        }
    }
}
