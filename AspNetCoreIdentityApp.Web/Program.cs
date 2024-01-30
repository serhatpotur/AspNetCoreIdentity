using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Core.Permissions;
using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.DataAccess.Models;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.DataAccess.Seeds;
using AspNetCoreIdentityApp.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.DataAccess");
    });
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = configuration["Authentication:Google:ClientID"];
    opt.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IstanbulAndAnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "Ýstanbul", "Ankara"); // city bilgisinde istanbul ve ankara olanlar belirlenen sayfaya eriþebilir
    });
    opt.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

    opt.AddPolicy("OrderBasicPermission", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
    });
    opt.AddPolicy("OrderAdvencedPermission", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Create, Permission.Order.Update);
    });
    opt.AddPolicy("OrderAdminPermission", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Delete);
    });
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); ///wwwroota eriþmek için yzdýk

builder.Services.AddIdentityExtensions();

builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityAppCookie";
    options.LoginPath = new PathString("/Auth/SignIn");
    options.LogoutPath = new PathString("/Auth/Logout2");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true; //kullanýcý sisteme girdiðinde süreyi +30 gün daha uzatýr;

});

var app = builder.Build();

//uygulama ayaða kaltýðýnda bir kez çalýþsýn ve silinsin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
