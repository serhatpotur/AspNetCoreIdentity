using AspNetCoreIdentityApp.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreIdentityApp.Core.ViewModels

{
    public class UserEditViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender{ get; set; }
        public string City { get; set; }
        public IFormFile? Picture { get; set; }

    }
}
