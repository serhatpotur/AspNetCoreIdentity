using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Business.Services
{
    public interface IAuthService
    {
        Task LogoutAsync();
    }
}
