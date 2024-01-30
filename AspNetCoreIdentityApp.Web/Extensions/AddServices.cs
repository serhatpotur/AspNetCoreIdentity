using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.DataAccess.Models;
using AspNetCoreIdentityApp.Business.Validations;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class AddServices
    {
        public static void AddIdentityExtensions(this IServiceCollection services)
        {
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //30 dk da bir cookie değeri ile dbde bulunan securitystamp değerini karşılaştırır. farklı ise logine atar
                options.ValidationInterval = TimeSpan.FromMinutes(30);
            });
            services.Configure<DataProtectionTokenProviderOptions>(options =>
              {
                  //resetpassword bağlatısı 1 saat geçerli olsun
                  options.TokenLifespan = TimeSpan.FromHours(1);
              });
            services.AddIdentity<AppUser, AppRole>(opt =>
             {
                 opt.User.RequireUniqueEmail = true;
                 opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); // 3 dk kitle
                 opt.Lockout.MaxFailedAccessAttempts = 3;//3 kez başarısız girerse kitle
             })
             .AddPasswordValidator<PasswordValidator>()
             .AddUserValidator<UserValidator>()
             .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
             .AddDefaultTokenProviders()
             .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
