using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AuthService(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
