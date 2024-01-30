using AspNetCoreIdentityApp.Core.Enums;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Business.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task<(bool, IEnumerable<IdentityError>)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var hasUser = await _userManager.FindByNameAsync(userName);

            var result = await _userManager.ChangePasswordAsync(hasUser, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            await _userManager.UpdateSecurityStampAsync(hasUser);
            await _signInManager.SignOutAsync();

            return (true, null);

        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var hasUser = await _userManager.FindByNameAsync(userName);

            var checkCurrentPassword = await _userManager.CheckPasswordAsync(hasUser, password);
            return checkCurrentPassword;
        }

        public async Task<AppUser> FindByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public SelectList GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<UserEditViewModel> GetUserEditViewModel(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
            return userEditViewModel;

        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userViewModel = new UserViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                PictureUrl = user.Picture
            };

            return userViewModel;
        }

        public async Task<(bool, IEnumerable<IdentityError>)> UpdateUserAsync(UserEditViewModel model, string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            currentUser.UserName = model.UserName;
            currentUser.Email = model.Email;
            currentUser.City = model.City;
            currentUser.BirthDate = model.BirthDate;
            currentUser.Gender = model.Gender;
            currentUser.PhoneNumber = model.PhoneNumber;
            if (model.Picture.Length > 0 && model.Picture != null)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(model.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "UserPictures").PhysicalPath, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await model.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }
            var result = await _userManager.UpdateAsync(currentUser);
            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            return (true, null);

        }
    }
}
