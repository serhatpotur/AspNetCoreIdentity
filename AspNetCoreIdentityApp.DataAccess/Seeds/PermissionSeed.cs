using AspNetCoreIdentityApp.Core.Permissions;
using AspNetCoreIdentityApp.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.DataAccess.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            var hasAdvencedRole = await roleManager.RoleExistsAsync("AdvencedRole");
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");
            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "BasicRole" });
                var basicRole = await roleManager.FindByNameAsync("BasicRole");
                await AddReadPermission(roleManager, basicRole);
            }
            if (!hasAdvencedRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "AdvencedRole" });
                var advencedRole = await roleManager.FindByNameAsync("AdvencedRole");
                await AddReadPermission(roleManager, advencedRole);
                await AddCreateOrUpdatePermission(roleManager, advencedRole);
            }
            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "AdminRole" });
                var adminRole = await roleManager.FindByNameAsync("AdminRole");
                await AddReadPermission(roleManager, adminRole);
                await AddCreateOrUpdatePermission(roleManager, adminRole);
                await AddDeletePermission(roleManager, adminRole);

            }
        }

        public static async Task AddReadPermission(RoleManager<AppRole> roleManager, AppRole appRole)
        {
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Stock.Read));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Order.Read));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Catalog.Read));
        }
        public static async Task AddCreateOrUpdatePermission(RoleManager<AppRole> roleManager, AppRole appRole)
        {
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Stock.Create));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Order.Create));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Catalog.Create));

            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Stock.Update));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Order.Update));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Catalog.Update));
        }
        public static async Task AddDeletePermission(RoleManager<AppRole> roleManager, AppRole appRole)
        {
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Stock.Delete));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Order.Delete));
            await roleManager.AddClaimAsync(appRole, new Claim("Permission", Permission.Catalog.Delete));
        }
    }
}
