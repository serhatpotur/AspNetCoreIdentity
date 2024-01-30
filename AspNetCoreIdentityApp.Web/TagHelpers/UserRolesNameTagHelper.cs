using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserRolesNameTagHelper : TagHelper
    {
        public string UserId { get; set; }
        private readonly UserManager<AppUser> _userManager;

        public UserRolesNameTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var userRoles = await _userManager.GetRolesAsync(user);

            var stringBuilder = new StringBuilder();

            foreach (var role in userRoles)
            {
                stringBuilder.Append(@$"<span class='badge bg-secondary mx-1'>{role.ToLower()}</span>");
            }

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }

    }
}
