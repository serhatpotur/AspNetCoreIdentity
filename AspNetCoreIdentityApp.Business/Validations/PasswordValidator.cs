using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Business.Validations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainUsername", Description = "Şifre içerisinde kullanıcı adı olamaz" });
            }


            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);

        }
    }
}
