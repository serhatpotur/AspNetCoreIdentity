using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Business.Services
{
    public interface IMemberService
    {
        Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName);
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<(bool, IEnumerable<IdentityError>)> ChangePasswordAsync(string userName,string oldPassword,string newPassword);
        Task<AppUser> FindByUserNameAsync(string userName);

        Task<UserEditViewModel> GetUserEditViewModel(string userName);
        SelectList GetGenderSelectList();
        Task<(bool, IEnumerable<IdentityError>)> UpdateUserAsync(UserEditViewModel model,string userName);

    }
}
