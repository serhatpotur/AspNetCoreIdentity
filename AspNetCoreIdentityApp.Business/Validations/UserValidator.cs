using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Business.Validations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var usernameStartWithNumeric = int.TryParse(user.UserName[0].ToString(), out _);

            if (usernameStartWithNumeric)
            {
                errors.Add(new()
                {
                    Code = "UsernameStartWithNumeric",
                    Description = "Kullanıcı Adı Rakam İle Başlayamaz"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
