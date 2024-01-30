using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.DataAccess.Models;
using AspNetCoreIdentityApp.Business.Services;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = new()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(user, model.Password);



            if (result.Succeeded)
            {
                var exchangeClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
                var claimResult = await _userManager.AddClaimAsync(user, exchangeClaim);
                if (!claimResult.Succeeded)
                {
                    ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                    return View();

                }
                TempData["Message"] = "Kayıt İşlemi Başarılı !";
                return RedirectToAction(nameof(AuthController.SignUp));
            }

            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {

            var hasUser = await _userManager.FindByNameAsync(model.UserName);
            if (hasUser == null)
            {
                TempData["SıgnInMessage"] = "Kullanıcı Bulunamadı !";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                TempData["SıgnInMessage"] = "Giriş İşlemi Başarılı !";
                return RedirectToAction("Index", "Member");
            }
            ModelState.AddModelErrorList(new List<string>() { "Kullanıcı adı veya şifre hatalı", $"Başarısız Giriş Sayısı : {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
            if (result.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Şifrenizi 3 kez yanlış girdiğiniz için sisteme 3 dakila boyunca giriş yapamazsınız" });
                return View();
            }




            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }
        public async Task Logout2()
        {
            await _authService.LogoutAsync();


        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var hasUser = await _userManager.FindByEmailAsync(model.Email);
            if (hasUser == null)
            {
                ModelState.AddModelErrorList(new List<string>() { "Bu maile ait bir kullanıcı bulunamadı" });
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            string passwordResetUrl = Url.Action("ResetPassword", "Auth", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            TempData["ForgotPasswordMessage"] = "Şifre yenileme bağlantısı e-mail adresinize gönderilmiştir";

            await _emailService.SendResetPasswordEmail(passwordResetUrl, hasUser.Email);
            ///kpyikslemrnufujm
            return RedirectToAction(nameof(AuthController.ForgotPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId; TempData["token"] = token;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var queryString = HttpContext.Request.QueryString.Value;

            var queryParameters = System.Web.HttpUtility.ParseQueryString(queryString);

            // userId ve Token parametrelerini al
            string userId = queryParameters["userId"];
            string token = queryParameters["Token"];

            if (userId == null || token == null)
                throw new Exception("Bir Hata Meydana geldi");


            var hasUser = await _userManager.FindByIdAsync(userId.ToString());
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı");
                return View();

            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler Uyuşmuyor");
                return View();

            }
            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString(), model.NewPassword);
            if (result.Succeeded)
                TempData["ResetPasswordMessage"] = "Şifre başarılı bir şekilde değiştirildi";

            else
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            return View();
        }

        public IActionResult GoogleLogin(string returnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Auth", new { returnUrl = returnUrl });
            var result = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);

            return new ChallengeResult("Google", result);
        }
        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            ExternalLoginInfo externalInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalInfo == null)
            {
                return RedirectToAction("SignIn", "Auth");

            }
            else
            {
                var result = await _signInManager.ExternalLoginSignInAsync(externalInfo.LoginProvider, externalInfo.ProviderKey, true);

                if (result.Succeeded)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    AppUser appUser = new();
                    appUser.Email = externalInfo.Principal.FindFirst(ClaimTypes.Email).Value;
                    string externalUserId = externalInfo.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                    if (externalInfo.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = externalInfo.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(' ', '-').ToLower() + externalUserId.Substring(0, 5).ToString();

                        appUser.UserName = userName;
                    }
                    else
                    {
                        appUser.UserName = externalInfo.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    var createResult = await _userManager.CreateAsync(appUser);
                    if (createResult.Succeeded)
                    {
                        var loginResult = await _userManager.AddLoginAsync(appUser, externalInfo);
                        if (loginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(appUser, true);
                            return Redirect(returnUrl);

                        }
                        else
                        {
                            ModelState.AddModelErrorList(loginResult.Errors);
                        }
                    }
                    else
                    {
                        ModelState.AddModelErrorList(createResult.Errors);

                    }
                }
            }

            return RedirectToAction("Error");
        }
    }
}
