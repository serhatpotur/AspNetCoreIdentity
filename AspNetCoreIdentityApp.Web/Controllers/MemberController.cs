using AspNetCoreIdentityApp.Core.Enums;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.DataAccess.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Business.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity.Name;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            UserViewModel userViewModel = await _memberService.GetUserViewModelByUserNameAsync(userName);

            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Tüm alanlar dolu olmalıdır");
                return View();
            }
            var hasUser = await _memberService.FindByUserNameAsync(userName);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı");
                return View();
            }
            var checkCurrentPassword = await _memberService.CheckPasswordAsync(userName, model.OldPassword);
            if (!checkCurrentPassword)
            {
                ModelState.AddModelError(string.Empty, "Mevcut şifreniz yanlış");
                return View();
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Yeni şifreler birbiri ile uyuşmuyor");
                return View();
            }
            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, model.OldPassword, model.NewPassword);
            if (!isSuccess)
            {

                ModelState.AddModelErrorList(errors);
                return View();
            }
            return RedirectToAction("SignIn", "Auth");

        }

        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.GenderList = _memberService.GetGenderSelectList();

            UserEditViewModel userEditViewModel = await _memberService.GetUserEditViewModel(userName);
            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }

            var (isSuccess,errors) = await _memberService.UpdateUserAsync(model, userName);
            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }


            TempData["UserEditMessage"] = "Bilgiler başarılı bir şekilde güncellendi";
            return RedirectToAction("SignIn", "Auth");

        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        public IActionResult UserClaims()
        {
            var userClaimList = User.Claims.Select(x => new UserClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value,
            }).ToList();
            return View(userClaimList);
        }

        [Authorize(Policy = "IstanbulAndAnkaraPolicy")]
        [HttpGet]
        public IActionResult IstanbulAndAnkaraPage()
        {
            return View();
        }
        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }
    }
}
