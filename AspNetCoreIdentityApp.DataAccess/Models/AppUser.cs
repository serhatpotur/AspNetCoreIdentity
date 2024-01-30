using AspNetCoreIdentityApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreIdentityApp.DataAccess.Models
{
    public class AppUser : IdentityUser
    {
        public string? City { get; set; }
       
        public string? Picture { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

    }

}
